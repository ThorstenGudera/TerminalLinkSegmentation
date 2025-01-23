using AvoidAGrabCutEasy.ProcOutline;
using Cache;
using ChainCodeFinder;
using GetAlphaMatte;
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmProcOutline : Form
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
        private DefaultSmoothenOP? _dsOP;
        private BoundaryMattingOP? _bmOP;
        private int _maxWidth = 100;
        private int _oW = 0;
        private int _iW = 0;
        private Stopwatch? _sw;
        private ClosedFormMatteOp? _cfop;
        private int _lastRunNumber;
        private List<TrimapProblemInfo> _trimapProblemInfos = new List<TrimapProblemInfo>();
        private Bitmap? _bmpRef;
        private GrabCutOp? _gc;
        private object _lockObject = new object();
        private List<List<Rectangle>>? _rectsList;
        private Bitmap? _bmpWork;
        private Bitmap? _bmpTrimap;
        private Bitmap? _bmpMatte;
        private List<Bitmap>? _excludedRegions;
        private List<Point>? _exclLocations;

        public Bitmap FBitmap
        {
            get
            {
                return this.helplineRulerCtrl1.Bmp;
            }
        }

        public frmProcOutline(Bitmap bmp, Bitmap bmpOrig, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = new Bitmap(bmp);
                _bmpBU = new Bitmap(bmp);
                this._bmpOrig = new Bitmap(bmpOrig);
                this.pictureBox1.Image = this._bmpOrig;
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

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {

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
                Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                this.toolStripStatusLabel2.BackColor = c;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {

        }

        private void helplineRulerCtrl1_Paint(object? sender, PaintEventArgs e)
        {
            if (this.cbShowAngles.Checked && this._rectsList != null && this._rectsList.Count > 0)
            {
                for (int i = 0; i < this._rectsList.Count; i++)
                {
                    List<Rectangle> rectangles = this._rectsList[i];

                    if (rectangles.Count > 0)
                    {
                        for (int j = 0; j < rectangles.Count; j++)
                        {
                            Rectangle rc = new Rectangle((int)(rectangles[j].X * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                (int)(rectangles[j].Y * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                (int)(rectangles[j].Width * this.helplineRulerCtrl1.Zoom),
                                (int)(rectangles[j].Height * this.helplineRulerCtrl1.Zoom));
                            using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Red)))
                                e.Graphics.FillRectangle(sb, rc);
                        }
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

        private void cmbZoom_SelectedIndexChanged(object? sender, EventArgs e)
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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap? bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void frmProcOutline_FormClosing(object sender, FormClosingEventArgs e)
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

                if (this.backgroundWorker3.IsBusy)
                    this.backgroundWorker3.CancelAsync();

                if (this.backgroundWorker4.IsBusy)
                    this.backgroundWorker4.CancelAsync();

                if (this.backgroundWorker5.IsBusy)
                    this.backgroundWorker5.CancelAsync();

                if (this.backgroundWorker6.IsBusy)
                    this.backgroundWorker6.CancelAsync();

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                if (this._bmpOrig != null)
                    this._bmpOrig.Dispose();
                if (this._bmpRef != null) //--> result
                    this._bmpRef.Dispose();
                if (this._bmpTrimap != null)
                    this._bmpTrimap.Dispose();
                if (this._bmpWork != null)
                    this._bmpWork.Dispose();
                if (this._bmpMatte != null)
                    this._bmpMatte.Dispose();
                if (this.pictureBox2.Image != null)
                    this.pictureBox2.Image.Dispose();

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

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "tAppBoundary");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
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

        private void btnJRem_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                Bitmap? bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);

                if (this.numJRem1.Value > 0)
                {
                    Bitmap? bTmp = Fipbmp.RemOutline(bWork, Math.Max((int)this.numJRem1.Value, 1), null);

                    Bitmap? bOld = bWork;
                    bWork = bTmp;
                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }
                }

                if (this.numJRem2.Value > 0 && this._bmpOrig != null && bWork != null)
                {
                    Bitmap? bTmp4 = Fipbmp.ExtOutline(bWork, this._bmpOrig, Math.Max((int)this.numJRem2.Value, 0), null);

                    if (bTmp4 != null)
                    {
                        Bitmap? bOld2 = bWork;
                        bWork = bTmp4;
                        if (bOld2 != null)
                        {
                            bOld2.Dispose();
                            bOld2 = null;
                        }
                    }
                }

                if (bWork != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bWork, this.helplineRulerCtrl1, "Bmp");

                    Bitmap? bC = new Bitmap(bWork);
                    this.SetBitmap(ref _bmpRef, ref bC);

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                        (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    _undoOPCache?.Add(bWork);
                }

                this._pic_changed = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.Cursor = Cursors.Default;
                this.SetControls(true);

                this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());
            }
        }

        private void btnPPGo_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);
                this.btnPPGo.Text = "Cancel";
                this.btnPPGo.Enabled = true;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                this.backgroundWorker1.RunWorkerAsync();
            }
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

        private void btnAlphaV_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker2.IsBusy)
            {
                this.backgroundWorker2.CancelAsync();
                return;
            }
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }
            if (this.backgroundWorker6.IsBusy)
            {
                this.backgroundWorker6.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                if (this._rectsList != null && this._rectsList.Count > 0)
                    this._rectsList.Clear();

                if (this.cbExpOutlProc.Checked)
                {
                    //closedFormMatte
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    this.btnAlphaV.Text = "Cancel";
                    this.btnAlphaV.Enabled = true;

                    this.numSleep.Enabled = this.label2.Enabled = this.numError.Enabled = this.label54.Enabled = true;

                    if (_sw == null)
                        _sw = new Stopwatch();
                    _sw.Reset();
                    _sw.Start();

                    this.btnOK.Enabled = this.btnCancel.Enabled = false;

                    /*
                    int windowSize = (int)this.numWinSz.Value;
                    double gamma = (double)this.numGamma.Value;
                    int normalDistToCheck = 10;   
                    double gamma2 = (double)this.numGamma2.Value;

                    this.backgroundWorker3.RunWorkerAsync(new object[] { windowSize, gamma, normalDistToCheck, gamma2 });
                    */

                    if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
                    {
                        int innerW = this._iW;
                        int outerW = this._oW;
                        bool redrawInner = this.cbRedrawInner.Checked;

                        bool editTrimap = this.cbEditTrimap.Checked;
                        Bitmap bWork = new Bitmap(this._bmpOrig);

                        double res = CheckWidthHeight(bWork, true, (double)this.numMaxSize.Value);
                        this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                        if (res > 1)
                        {
                            Bitmap? bOld = bWork;
                            bWork = ResampleDown(bWork, res);
                            if (bOld != null)
                            {
                                bOld.Dispose();
                                bOld = null;
                            }
                        }

                        if (cbHalfSize.Checked)
                        {
                            Bitmap bWork2 = ResampleBmp(bWork, 2);

                            Bitmap? bOld = bWork;
                            bWork = bWork2;
                            bOld.Dispose();
                            bOld = null;
                        }

                        double factor = this.cbHalfSize.Checked ? 2.0 : 1.0;
                        factor *= (res > 1) ? res : 1.0;

                        //changed
                        Bitmap bTrimap = this._bmpTrimap == null ? new Bitmap(bWork.Width, bWork.Height) : new Bitmap(this._bmpTrimap);

                        Size sTest = new((int)Math.Ceiling(this.helplineRulerCtrl1.Bmp.Width / factor),
                            (int)Math.Ceiling(this.helplineRulerCtrl1.Bmp.Height / factor));

                        if (this._bmpTrimap != null) //has existing trimap the correct size?
                            if (sTest.Width != this._bmpTrimap.Width || sTest.Height != this._bmpTrimap.Height)
                            {
                                Bitmap? bTmp2 = new Bitmap(sTest.Width, sTest.Height);
                                using Graphics gx = Graphics.FromImage(bTmp2);
                                gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                                gx.DrawImage(this._bmpTrimap, 0, 0, bTmp2.Width, bTmp2.Height);
                                this.SetBitmap(ref this._bmpTrimap, ref bTmp2);
                                if (bTmp2 != null)
                                {
                                    Bitmap? bOld2 = bTrimap;
                                    bTrimap = new Bitmap(bTmp2);
                                    if (bOld2 != null)
                                        bOld2.Dispose();
                                    bOld2 = null;
                                }
                            }

                        if (this._bmpTrimap == null)
                        {
                            innerW = (int)Math.Max(Math.Ceiling(innerW / factor), 1);
                            outerW = (int)Math.Max(Math.Ceiling(outerW / factor), 1);

                            Bitmap? bW = new Bitmap(this.helplineRulerCtrl1.Bmp);

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
                                    List<ChainCode> c = GetBoundary(bTrimapTmp);
                                    ChainFinder cf = new ChainFinder();
                                    cf.AllowNullCells = true;

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
                        }

                        if (bTrimap != null)
                        {
                            Bitmap trWork = bTrimap;

                            if (editTrimap)
                            {
                                using (frmEditTrimap frm = new frmEditTrimap(trWork, bWork, factor))
                                {
                                    Bitmap? bmp = null;

                                    frm.numError.Visible = frm.label54.Visible = true;
                                    frm.numError.Value = this.numError.Value;

                                    if (frm.ShowDialog() == DialogResult.OK && frm.FBitmap != null)
                                    {
                                        bmp = new Bitmap(frm.FBitmap);

                                        this.numError.Value = frm.numError.Value;
                                        frm.numError.Visible = frm.label54.Visible = false;

                                        Bitmap? bOld2 = trWork;
                                        trWork = bmp;
                                        if (bOld2 != null)
                                        {
                                            bOld2.Dispose();
                                            bOld2 = null;
                                        }

                                        if (trWork != null)
                                        {
                                            Image? iOld = this.pictureBox2.Image;
                                            this.pictureBox2.Image = new Bitmap(trWork);
                                            this.pictureBox2.Refresh();

                                            if (iOld != null)
                                            {
                                                iOld.Dispose();
                                                iOld = null;
                                            }
                                        }
                                    }
                                }
                            }

                            if (trWork != null)
                            {
                                ClosedFormMatteOp cfop = new ClosedFormMatteOp(bWork, trWork);
                                BlendParameters bParam = new BlendParameters();
                                bParam.MaxIterations = (int)this.numMaxRestarts.Value; //for GaussSeidel set this to 10000 or so
                                bParam.InnerIterations = 35; //maybe 25 will do
                                bParam.DesiredMaxLinearError = (double)this.numError.Value;
                                bParam.Sleep = this.numSleep.Value > 0 ? true : false;
                                bParam.SleepAmount = (int)this.numSleep.Value;
                                bParam.BGW = this.backgroundWorker4;
                                cfop.BlendParameters = bParam;
                                this._cfop = cfop;

                                this._cfop.ShowProgess += Cfop_UpdateProgress;
                                this._cfop.ShowInfo += _cfop_ShowInfo;
                                this._cfop.ShowInfoOuter += Cfop_ShowInfoOuter;

                                bool scalesPics = this.cbSlices.Checked;
                                int scales = scalesPics ? rb4.Checked ? 4 : 16 : 0;
                                int overlap = 32;
                                bool interpolated = this.cbInterpolated.Checked;
                                bool forceSerial = this.cbForceSerial.Checked;
                                bool group = false;
                                int groupAmountX = scalesPics ? 1 : 0; //we dont use grouping, so set it simply to 1
                                int groupAmountY = scalesPics ? 1 : 0;
                                int maxSize = this.cbInterpolated.Checked ? (int)this.numMaxSize.Value * 2 : (int)this.numMaxSize.Value;
                                bool trySingleTile = /*scalesPics ? false : this.cbHalfSize.Checked ? true :*/ bWork.Width * bWork.Height < maxSize ? true : false;
                                bool verifyTrimaps = false;

                                Bitmap? tr = new Bitmap(trWork);
                                Bitmap? bWrk = new Bitmap(bWork);
                                if (tr != null && bWrk != null)
                                {
                                    this.SetBitmap(ref this._bmpWork, ref bWrk);
                                    this.SetBitmap(ref this._bmpTrimap, ref tr);
                                }

                                this.backgroundWorker4.RunWorkerAsync(new object[] { 1 /*GMRES_r; 0 is GaussSeidel*/, scalesPics, scales, overlap,
                                    interpolated, forceSerial, group, groupAmountX, groupAmountY, maxSize, bWork, trWork,
                                    trySingleTile, verifyTrimaps });
                            }
                        }
                    }
                }
                else
                {
                    if (this.cmbMethodMode != null && this.cmbMethodMode.SelectedItem != null)
                    {
                        string? d = this.cmbMethodMode.SelectedItem?.ToString();
                        if (d != null)
                        {
                            MethodMode mm = (MethodMode)System.Enum.Parse(typeof(MethodMode), d);

                            bool restoreDefects = this.cbRestoreDefects.Checked;
                            double gamma2 = (double)this.numGamma2.Value;
                            float opacity = (float)this.numOpacity.Value;
                            double wMin = (double)this.numWMin.Value;
                            double wMax = (double)this.numWMax.Value;
                            int whFactor = (int)this.numWHFactor.Value;
                            bool desaturate = this.cbDesaturate.Checked;

                            if (mm == MethodMode.ModeFeather && this.cmbBlendType.SelectedItem != null)
                            {
                                string? b = this.cmbBlendType.SelectedItem.ToString();
                                if (b != null)
                                {
                                    //Feather (extended)
                                    BlendType bt = (BlendType)System.Enum.Parse(typeof(BlendType), b.ToString());
                                    this.backgroundWorker2.RunWorkerAsync(new object[] { bt, restoreDefects, gamma2, opacity, wMax, whFactor, wMin, desaturate });
                                }
                            }
                            else
                            {
                                //Run a grabcutWithoutMinCut again with current segmentation picture as fg and a stronger threshold
                                //compare and process the differences
                                if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
                                {
                                    this.Cursor = Cursors.WaitCursor;
                                    this.SetControls(false);

                                    this.toolStripProgressBar1.Value = 0;
                                    this.toolStripProgressBar1.Visible = true;

                                    this.btnAlphaV.Text = "Cancel";
                                    this.btnAlphaV.Enabled = true;

                                    int innerW = this._iW;
                                    int outerW = this._oW;

                                    Bitmap bOrig = new Bitmap(this._bmpOrig);
                                    Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);

                                    int gmm_comp = 2;
                                    double gamma = (double)50.0;
                                    int numIters = 1;
                                    bool rectMode = true;
                                    Rectangle r = GetR(bWork, this._oW);
                                    bool skipInit = false;
                                    bool workOnPaths = false;
                                    bool gammaChanged = false;
                                    int intMult = 1;
                                    bool useEightAdj = false;
                                    bool useTh = true;
                                    double th = (double)this.numTh.Value;
                                    double resPic = CheckWidthHeight(bWork, true, 1200);
                                    bool initWKpp = true;
                                    bool multCapacitiesForTLinks = true;
                                    double multTLinkCapacity = 2.0;
                                    bool castTLInt = true;
                                    bool getSourcePart = false;
                                    ListSelectionMode selMode = (ListSelectionMode)0;
                                    bool scribbleMode = false;
                                    double probMult1 = 1.0;
                                    double kmInitW = 2.0;
                                    double kmInitH = 2.0;
                                    bool setPFGToFG = false;
                                    double numItems = 0;
                                    double numCorrect = 0;
                                    double numItems2 = 0;
                                    double numCorrect2 = 0;
                                    bool skipLearn = false;

                                    Rectangle clipRect = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                                    bool dontFillPath = true;
                                    bool drawNumComp = true;
                                    int comp = 1000;

                                    int blur = (int)this.numBlur.Value;
                                    int alphaStartValue = (int)this.numAlphaStart.Value;
                                    bool doBlur = this.cbBlur.Checked;

                                    this.backgroundWorker6.RunWorkerAsync(new object[] { bWork, bOrig, innerW, outerW,
                                    gmm_comp, gamma, numIters, rectMode, r ,skipInit, workOnPaths,
                                    gammaChanged, intMult, useEightAdj, useTh, th, resPic,
                                    initWKpp, multCapacitiesForTLinks, multTLinkCapacity, castTLInt,
                                    getSourcePart, selMode, scribbleMode, probMult1,
                                    kmInitW, kmInitH, setPFGToFG, numItems, numCorrect,
                                    numItems2, numCorrect2, skipLearn, clipRect, dontFillPath,
                                    drawNumComp, comp, blur, alphaStartValue, doBlur, restoreDefects,
                                    gamma2, opacity, wMax, whFactor, wMin, desaturate });
                                }
                            }
                        }
                    }
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

        private void _cfop_ShowInfo(object? sender, string e)
        {
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
            this.backgroundWorker4.ReportProgress((int)(e.CurrentProgress / e.ImgWidthHeight * 100));
        }

        private void frmProcOutline_Load(object sender, EventArgs e)
        {
            foreach (string z in System.Enum.GetNames(typeof(BlendType)))
                this.cmbBlendType.Items.Add(z.ToString());

            this.cmbBlendType.SelectedIndex = 1;

            foreach (string z in System.Enum.GetNames(typeof(MethodMode)))
                this.cmbMethodMode.Items.Add(z.ToString());

            this.cmbMethodMode.SelectedIndex = 1;

            this.rbBoth.Checked = true;
            //this.cbBayes.Checked = true;

            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());

            DisableBoundControls(this.cbExpOutlProc.Checked);
            this.btnOnlyRestore.Enabled = false;

            this.cbExpOutlProc.Checked = true;

            if (this.helplineRulerCtrl1.Bmp != null)
                this.numMaxSize.Value = (decimal)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control? ct in this.splitContainer1.Panel2.Controls)
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

                                    if (c is GroupBox)
                                    {
                                        c.Enabled = true;
                                        GroupBox g = (GroupBox)c;
                                        foreach (Control c1 in g.Controls)
                                        {
                                            if (!(c1 is Button) && !(c1.Name == "numSleep") && !(c1.Name == "numError"))
                                                c1.Enabled = e;

                                            if (c1.Name == "numSleep")
                                                c1.Enabled = true;

                                            if (c1.Name == "numError")
                                                c1.Enabled = true;
                                        }
                                    }
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

                            if (c is GroupBox)
                            {
                                c.Enabled = true;
                                GroupBox g = (GroupBox)c;
                                foreach (Control c1 in g.Controls)
                                {
                                    if (!(c1 is Button) && !(c1.Name == "numSleep") && !(c1.Name == "numError"))
                                        c1.Enabled = e;

                                    if (c1.Name == "numSleep")
                                        c1.Enabled = true;

                                    if (c1.Name == "numError")
                                        c1.Enabled = true;
                                }
                            }
                        }
                    }
                }

                this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl1.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                Bitmap? bmp = null;

                DefaultSmoothenOP dsOP = new DefaultSmoothenOP(this.helplineRulerCtrl1.Bmp, this._bmpOrig);
                //dsOP.ShowInfo += _gc_ShowInfo;
                dsOP.BGW = this.backgroundWorker1;
                dsOP.Init((double)this.numPPEpsilon.Value, (double)this.numPPEpsilon2.Value, this.cbPPRemove.Checked, (int)this.numPPRemove.Value, (int)this.numPPRemove2.Value, this.cbApproxLines.Checked);
                if (this.cbPPCleanOutline.Checked)
                    dsOP.CleanOutline((int)this.numPPPixelDepthOuter.Value, (int)this.numPPThresholdOuter.Value, (int)this.numPPPixelDepthInner.Value, (int)this.numPPThresholdInner.Value,
                        this.cbRemArea.Checked, (int)this.numPPMinAllowedArea.Value, (int)this.numPPCleanAmount.Value);
                else
                {
                    if (this.cbRemArea.Checked)
                        dsOP.RemAreas((int)this.numPPMinAllowedArea.Value);
                }
                dsOP.ComputeOutlineInfo();
                bmp = dsOP.ComputePic(false, 0.0f, this.cbOutlinesAsCurves.Checked, (float)this.numTension.Value, false, 1.0f);
                if (bmp != null && this.cbReShift.Checked)
                {
                    Bitmap? bOld = bmp;
                    bmp = dsOP.ReShiftShapes(bmp, (int)this.numShiftX.Value, (int)this.numShiftY.Value);
                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }
                }
                this._dsOP = dsOP;

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
            Bitmap? bmp = null;

            if (e.Result != null)
                bmp = (Bitmap)e.Result;

            if (bmp != null)
            {
                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                Bitmap? bC = new Bitmap(bmp);
                this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bmp);
            }

            if (this._dsOP != null)
            {
                //this._dsOP.ShowInfo -= _gc_ShowInfo;
                this._dsOP.Dispose();
            }

            this.btnPPGo.Text = "Go";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

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

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            Bitmap? bmp = null;

            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                BlendType bt = (BlendType)o[0];
                bool restoreDefects = (bool)o[1];
                double gamma = (double)o[2];
                float opacity = (float)o[3];
                double wMax = (double)o[4];
                int whFactor = (int)o[5];
                double wMin = (double)o[6];
                bool desaturate = (bool)o[7];

                if (this._bmpOrig != null)
                {
                    BoundaryMattingOP bmOP = new BoundaryMattingOP(this.helplineRulerCtrl1.Bmp, this._bmpOrig);
                    //bmOP.ShowInfo += _gc_ShowInfo;
                    bmOP.BGW = this.backgroundWorker2;

                    bmOP.Init((int)this.numNormalDist.Value, (int)this.numBoundInner.Value, (int)this.numBoundOuter.Value,
                        (float)Math.Min(this.numColDistDist.Value, this.numBoundOuter.Value), (double)this.numAlphaStart.Value, bt, desaturate);

                    ColorSource cs = ColorSource.OuterPixels;
                    double numFactorOuterPx = (double)this.numFactorOuterPx.Value;
                    int blur = (int)this.numBlur.Value;

                    if (!this.cbBlur.Checked)
                        blur = 0;

                    if (this.rbBoundary.Checked)
                        cs = ColorSource.Boundary;
                    else if (this.rbBoth.Checked)
                        cs = ColorSource.Both;

                    bmp = bmOP.ProcessImage(cs, numFactorOuterPx, blur);

                    this._bmOP = bmOP;

                    if (restoreDefects && bmp != null)
                        RevisitConvexDefects(this.helplineRulerCtrl1.Bmp, bmp, gamma, opacity, wMax, whFactor, wMin);

                    e.Result = bmp;
                }
            }
        }

        //what we are doing here is to gather all angles that are smaller than a given value, because the "defects" are
        //visible after feathering at angles in the outline that are small. Then we simply get a small Bitmap from the
        //input image around that point, prcess this a bit and draw this in the result image. 
        private void RevisitConvexDefects(Bitmap bPrevious, Bitmap bmp, double gamma = 1.0, float opacity = 1.0f, double wMax = 180.0, int whFactor = 4, double wMin = 0)
        {
            //Get all connected components
            List<ChainCode> c = GetBoundary(bPrevious);
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;

            c = c.OrderByDescending(a => a.Coord.Count).ToList();

            if (this._rectsList == null)
                this._rectsList = new List<List<Rectangle>>();
            this._rectsList.Clear();

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

                    //now get a List of all Bitmaps to be used in this ChainCode, so we can draw all images (for the current ChainCode) in one pass
                    List<Tuple<Point, Bitmap>> bmps = new List<Tuple<Point, Bitmap>>();

                    //Size of bitmap
                    int wh = (this._oW + this._iW) * whFactor + 1;
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

                this._rectsList.Add(rects);
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

                Bitmap? bC = new Bitmap(bmp);
                this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bmp);
            }

            if (this._bmOP != null)
            {
                //this._bmOP.ShowInfo -= _gc_ShowInfo;
                this._bmOP.Dispose();
            }

            this.btnPPGo.Text = "Go";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

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

        private void cbExpOutlProc_CheckedChanged(object sender, EventArgs e)
        {
            DisableBoundControls(this.cbExpOutlProc.Checked);
        }

        private void DisableBoundControls(bool ch)
        {
            for (int i = 0; i < this.groupBox4.Controls.Count; i++)
                if (!(this.groupBox4.Controls[i] is GroupBox) && !(this.groupBox4.Controls[i] is Button))
                    this.groupBox4.Controls[i].Enabled = !ch;

            this.pictureBox1.Enabled = this.pictureBox2.Enabled = true;

            this.label45.Enabled = this.label46.Enabled = this.numBoundOuter.Enabled = this.numBoundInner.Enabled = true;
            this.label52.Enabled = this.cbBlur.Enabled = this.numBlur.Enabled = !ch; //maybe this changes
            this.label54.Enabled = ch;
            this.cbHalfSize.Enabled = this.numError.Enabled = ch;
            this.label9.Enabled = this.numMaxSize.Enabled = this.btnResVals.Enabled = ch;
            this.label4.Enabled = this.numTh.Enabled = !ch;
            this.cbRedrawInner.Enabled = this.cbEditTrimap.Enabled = ch;
            this.cbUseExistingTrimap.Enabled = ch;

            cmbMethodMode_SelectedIndexChanged(this.cmbMethodMode, new EventArgs());
            cbRestoreDefects_CheckedChanged(this.cbRestoreDefects, new EventArgs());

            int maxWidth = this._maxWidth;
            int oW = (int)this.numBoundOuter.Value;
            int iW = (int)this.numBoundInner.Value;

            if (oW + iW > maxWidth)
            {
                int diff = oW + iW - maxWidth;

                if (diff > 0)
                {
                    if (oW >= diff)
                        oW -= diff;
                    else
                    {
                        iW -= Math.Max(diff - oW, 0);
                        oW = 0;
                    }
                }
            }

            this._oW = oW;
            this._iW = iW;
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

        private Bitmap ExtendOutlineEx(Bitmap bmp, int outerW, bool dontFill, bool drawPath)
        {
            Bitmap bOut = new Bitmap(bmp.Width, bmp.Height);

            List<ChainCode> lInner = GetBoundary(bmp);

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
                if (lInner.Count > 0)
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

        private Bitmap ExtendOutlineEx2(Bitmap bmp, int outerW, bool dontFill, bool drawPath)
        {
            Bitmap bOut = new Bitmap(bmp.Width, bmp.Height);

            List<ChainCode> lInner = GetBoundary(bmp);

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
                                    {
                                        gx.FillPath(tb, gp);

                                        using (Pen pen = new Pen(tb, 1))
                                            gx.DrawPath(Pens.Gray, gp);

                                        if (drawPath && outerW > 0)
                                        {
                                            try
                                            {
                                                using (Pen pen = new Pen(tb, outerW))
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
            }

            if (dontFill)
                if (lInner.Count > 0)
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
                                            if (drawPath)
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

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                int windowSize = (int)o[0];
                double gamma = (double)o[1];
                int normalDistToCheck = (int)o[2];
                double gamma2 = (double)o[3];

                if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
                {
                    int innerW = this._iW;
                    int outerW = this._oW;

                    Bitmap bTrimap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                    using (Bitmap bForeground = RemoveOutlineEx(this.helplineRulerCtrl1.Bmp, innerW, true))
                    using (Bitmap bUnknown = ExtendOutlineEx(this.helplineRulerCtrl1.Bmp, outerW, true, false))
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

                    Bitmap bWork = new Bitmap(this._bmpOrig);
                    Bitmap trWork = bTrimap;

                    e.Result = ExperimentalOutlineProc(bWork, trWork, windowSize, gamma, gamma2, normalDistToCheck);
                    return;
                }
            }
        }

        private Bitmap? ExperimentalOutlineProc(Bitmap bOrig, Bitmap trWork, int windowSize, double gamma, double gamma2, int normalDistToCheck)
        {
            Bitmap fg = new Bitmap(this.helplineRulerCtrl1.Bmp);

            BoundaryMattingOP bMOP = new BoundaryMattingOP(fg, bOrig);
            Bitmap? bRes = bMOP.ExperimentalOutlineProc(trWork, this._iW, this._oW, windowSize, gamma, gamma2, normalDistToCheck, this.backgroundWorker3);
            bMOP.Dispose();

            return bRes;
        }

        private void backgroundWorker3_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (this._sw != null)
                if (!InvokeRequired)
                {
                    this.Text = "frmProcOutline";
                    this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
                    if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                        this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
                }
                else
                    this.Invoke(new Action(() =>
                    {
                        this.Text = "frmProcOutline";
                        if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                        {
                            this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
                            this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
                        }
                    }));
        }

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bRes = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bRes, this.helplineRulerCtrl1, "Bmp");

                Bitmap? bC = new Bitmap(bRes);
                this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bRes);

            }

            this.btnPPGo.Text = "Go";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

            this.btnAlphaV.Text = "Go";

            this._pic_changed = true;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();

            this.backgroundWorker3.Dispose();
            this.backgroundWorker3 = new BackgroundWorker();
            this.backgroundWorker3.WorkerReportsProgress = true;
            this.backgroundWorker3.WorkerSupportsCancellation = true;
            this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            this.backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
            this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;

            this._sw?.Stop();
            this.Text = "frmProcOutline";
            if (this._sw != null)
                this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
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

        private void btmSetBU_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.toolStripStatusLabel4.Text = "working...";
                this.statusStrip1.Refresh();
                Bitmap? bC = new Bitmap(this.helplineRulerCtrl1.Bmp);
                this.SetBitmap(ref this._bmpBU, ref bC);
                this.toolStripStatusLabel4.Text = "done";
            }
        }

        private void numBoundOuter_ValueChanged(object sender, EventArgs e)
        {
            DisableBoundControls(this.cbExpOutlProc.Checked);
        }

        private void numBoundInner_ValueChanged(object sender, EventArgs e)
        {
            DisableBoundControls(this.cbExpOutlProc.Checked);
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
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
                    groupAmountY, maxSize, bWork, trWork, trySingleTile, verifyTrimaps, Environment.TickCount, this.backgroundWorker4);
            }
            else
                e.Result = null;
        }

        private void backgroundWorker4_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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

                        frmEdgePic frm4 = new frmEdgePic(bmp, this.helplineRulerCtrl1.Bmp.Size);
                        frm4.Text = "Alpha Matte";
                        frm4.ShowDialog();

                        if (this._bmpOrig != null)
                        {
                            b2 = bmp;
                            bmp = GetAlphaBoundsPic(this._bmpOrig, bmp, true);
                            b2.Dispose();
                            b2 = null;
                        }
                    }
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                    Bitmap? bC = new Bitmap(bmp);
                    this.SetBitmap(ref _bmpRef, ref bC);

                    this.toolStripDropDownButton1.Enabled = true;

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                        (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    _undoOPCache?.Add(bmp);
                }

                if (this._cfop != null)
                {
                    this._cfop.ShowProgess -= Cfop_UpdateProgress;
                    this._cfop.ShowInfo -= _cfop_ShowInfo;
                    this._cfop.ShowInfoOuter -= Cfop_ShowInfoOuter;
                    this._cfop.Dispose();
                }

                this.btnAlphaV.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

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
                this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
                this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;

                this._sw?.Stop();
                this.Text = "frmProcOutline";
                if (this._sw != null)
                    this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
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

        private void btnSetGamma_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker5.IsBusy)
            {
                this.backgroundWorker5.CancelAsync();
                return;
            }

            //test, if _bmpRef could be trashed class-wide
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bC = new Bitmap(this.helplineRulerCtrl1.Bmp);
                this.SetBitmap(ref _bmpRef, ref bC);
            }

            if (this.helplineRulerCtrl1.Bmp != null && this._bmpRef != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnSetGamma.Enabled = true;
                this.btnSetGamma.Text = "Cancel";

                double gamma = (double)this.numGamma.Value;

                Bitmap b = this._bmpRef;
                bool redrawExcluded = false;

                string? c = this.CachePathAddition;
                if (c != null && this.cbExcludeRegions.Checked)
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

                this.backgroundWorker5.RunWorkerAsync(new object[] { b, gamma, redrawExcluded });
            }
        }

        private void backgroundWorker5_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (this._bmpRef != null && e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = (Bitmap)o[0];
                double gamma = (double)o[1];
                bool redrawExcluded = (bool)o[2];

                Bitmap? b = GetAlphaBoundsPic(bmp, gamma);
                if (b != null)
                    e.Result = new object[] { b, redrawExcluded };
            }
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

                this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

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

        private unsafe void backgroundWorker6_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap? bWork = (Bitmap)o[0];
                Bitmap? bOrig = (Bitmap)o[1];
                int innerW = (int)o[2];
                int outerW = (int)o[3];
                int gmm_comp = (int)o[4];
                double gamma = (double)o[5];
                int numIters = (int)o[6];
                bool rectMode = (bool)o[7];
                Rectangle r = (Rectangle)o[8];
                bool skipInit = (bool)o[9];
                bool workOnPaths = (bool)o[10];
                bool gammaChanged = (bool)o[11];
                int intMult = (int)o[12];
                bool quick = true;
                bool useEightAdj = (bool)o[13];
                bool useTh = (bool)o[14];
                double th = (double)o[15];
                double resPic = (double)o[16];
                bool initWKpp = (bool)o[17];
                bool multCapacitiesForTLinks = (bool)o[18];
                double multTLinkCapacity = (double)o[19];
                bool castTLInt = (bool)o[20];
                bool getSourcePart = (bool)o[21];
                ListSelectionMode selMode = (ListSelectionMode)o[22];
                bool scribbleMode = (bool)o[23];
                double probMult1 = (double)o[24];
                double kmInitW = (double)o[25];
                double kmInitH = (double)o[26];
                bool setPFGToFG = (bool)o[27];
                double numItems = (double)o[28];
                double numCorrect = (double)o[29];
                double numItems2 = (double)o[30];
                double numCorrect2 = (double)o[31];
                bool skipLearn = (bool)o[32];
                Rectangle clipRect = (Rectangle)o[33];
                bool dontFillPath = (bool)o[34];
                bool drawNumComp = (bool)o[35];
                int comp = (int)o[36];
                int blur = (int)o[37];
                int alphaStartValue = (int)o[38];
                bool doBlur = (bool)o[39];
                bool restoreDefects = (bool)o[40];
                double gamma2 = (double)o[41];
                float opacity = (float)o[42];
                double wMax = (double)o[43];
                int whFactor = (int)o[44];
                double wMin = (double)o[45];
                bool desaturate = (bool)o[46];

                //resize the input bmp
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

                Bitmap? bTrimap = new Bitmap(bWork.Width, bWork.Height);
                Bitmap? bInner = null;

                using (Bitmap bForeground = RemoveOutlineEx(bWork, innerW, true))
                using (Bitmap bUnknown = ExtendOutlineEx2(bWork, outerW, true, false))
                {
                    using (Graphics gx = Graphics.FromImage(bTrimap))
                    {
                        gx.SmoothingMode = SmoothingMode.None;
                        gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                        gx.Clear(Color.Black);
                        gx.DrawImage(bUnknown, 0, 0);
                        gx.DrawImage(bForeground, 0, 0);
                    }

                    bInner = GetInnerFGPic(bWork, bForeground);
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
                        Rc = r,
                        BGW = this.backgroundWorker6,
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
                        NumCorrect2 = numCorrect2
                    };

                    this._gc.ShowInfo += _gc_ShowInfo;
                }

                //now do the initialization
                //eg create the mask, preclassify the imagedata, compute the smootheness function and init the Gmms
                if (!skipInit)
                {
                    int it = this._gc.InitWithTrimap(bTrimap);

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

                    if (!workOnPaths && this._gc.ScribbleMode && this._gc.Scribbles != null && this._gc.Scribbles.Count > 0)
                    {
                        if (!this._gc.RectMode)
                            r = new Rectangle(0, 0, bWork.Width, bWork.Height);
                        this._gc.ReInitScribbles();
                    }
                }

                //now do the work ...
                int l = this._gc.RunBoundary();

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
                Bitmap? bRes = new Bitmap(bWork.Width, bWork.Height);

                int[,]? m = this._gc.Mask;

                if ((scribbleMode && !rectMode) || workOnPaths)
                    r = new Rectangle(0, 0, bWork.Width, bWork.Height);

                //lock the bmps for fast processing
                BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmWork = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int w = bTmp.Width;
                int h = bTmp.Height;
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

                //write the data
                if (m != null)
                {
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

                //now do some analysis of the result, to be able to redraw the resultpic with a different set of components
                Bitmap bResCopy = new Bitmap(bRes);
                Bitmap bCTransp = new Bitmap(bRes);

                //BU of the original result
                //this.SetBitmap(ref this._bResCopy, ref bResCopy);

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
                }

                //get a backup - for later use - of this bmp with the transparent paths
                //this.SetBitmap(ref this._bResCopyTransp, ref bCTransp);

                bTmp.Dispose();

                if (bInner != null)
                {
                    Bitmap? bDiff = GetDiff(bRes, bInner);

                    using (Graphics gx = Graphics.FromImage(bDiff))
                        gx.DrawImage(bInner, 0, 0);

                    if (doBlur)
                    {
                        Fipbmp fip = new Fipbmp();
                        fip.SmoothByAveragingA(bDiff, blur, this.backgroundWorker6);
                    }

                    BoundaryMattingOP bmOP = new BoundaryMattingOP();
                    //bmOP.Feather(bDiff, (int)Math.Max(innerW * (resPic > 1 ? resPic : 1), 1), alphaStartValue, innerW);
                    if (desaturate)
                        bmOP.SetDesaturate(desaturate);
                    bmOP.Feather(bDiff, (int)Math.Max(innerW * (resPic > 1 ? resPic : 1), 1) + 2, alphaStartValue, innerW);
                    bmOP.Dispose();

                    using (Graphics gx = Graphics.FromImage(bDiff))
                        gx.DrawImage(bInner, 0, 0);

                    if (resPic > 1)
                    {
                        Bitmap bOld = bRes;

                        bRes = ResampleUp(bDiff, resPic, bU2, dontFillPath, false);
                        //bRes = ResampleUp(bRes, resPic, bU2, dontFillPath, false);
                        //Bitmap bRCopy = ResampleUp(this._bResCopy, resPic, bU2, dontFillPath, false);
                        //Bitmap bRCopy2 = ResampleUp(this._bResCopyTransp, resPic, bU2, true, true);

                        //this.SetBitmap(ref this._bResCopy, ref bRCopy);
                        //this.SetBitmap(ref this._bResCopyTransp, ref bRCopy2);

                        //bU2.Dispose(); //--> is disposed in ResampleUp

                        if (bOld != null)
                            bOld.Dispose();
                    }
                    else if (resPic <= 1)
                    {
                        Bitmap bOld = bRes;
                        bRes = new Bitmap(bDiff);
                        if (bOld != null)
                            bOld.Dispose();
                    }
                    //else if (resPic == 1)
                    //{
                    //    //set the list of all found paths [chains] to re_use later
                    //    List<ChainCode> allChains = GetBoundary(this._bResCopyTransp);
                    //    this._allChains = allChains;

                    //    if (allChains != null && allChains.Count > 0)
                    //    {
                    //        int area = allChains.Sum(a => a.Area);
                    //        int pxls = w * h;

                    //        int fc = pxls / area;

                    //        //if we have almost no output, maybe the initialization of the Gmms hasn't been good enough to receive a reasonable result
                    //        //so restart with some different KMeans initialization, if wanted
                    //        if (fc > 1000)
                    //            if (MessageBox.Show("Amount pixels to segmented area ratio is " + fc.ToString() + "." +
                    //                "Rerun with different Initialization of the Gmms?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    //            {
                    //                this._restartDiffInit = true;
                    //            }
                    //    }
                    //}

                    bDiff.Dispose();
                    bDiff = null;
                }

                if (bInner != null)
                    bInner.Dispose();
                bInner = null;
                bWork.Dispose();
                bWork = null;
                bOrig.Dispose();
                bOrig = null;
                bTrimap.Dispose();
                bTrimap = null;

                //our result pic
                if (bRes != null)
                {
                    Bitmap? bmp2 = GetAlphaBoundsPic(bRes, 2); //bRes;

                    bRes.Dispose();
                    bRes = null;

                    if (restoreDefects && bmp2 != null)
                        RevisitConvexDefects(this.helplineRulerCtrl1.Bmp, bmp2, gamma2, opacity, wMax, whFactor, wMin);

                    e.Result = bmp2;
                }
            }
        }

        private unsafe Bitmap GetDiff(Bitmap bRes, Bitmap bInner)
        {
            int w = bRes.Width;
            int h = bRes.Height;
            Bitmap bOut = new Bitmap(w, h);

            BitmapData bmData = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmWork = bRes.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmInner = bInner.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = bmData.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmData.Scan0;
                p += y * stride;
                byte* pWork = (byte*)bmWork.Scan0;
                pWork += y * stride;
                byte* pInner = (byte*)bmInner.Scan0;
                pInner += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pInner[3] > 0 || pWork[3] > 0)
                    {
                        p[0] = (byte)Math.Abs(((int)pInner[0] - (int)pWork[0]));
                        p[1] = (byte)Math.Abs(((int)pInner[1] - (int)pWork[1]));
                        p[2] = (byte)Math.Abs(((int)pInner[2] - (int)pWork[2]));
                        p[3] = Math.Max(pInner[3], pWork[3]);
                    }

                    p += 4;
                    pWork += 4;
                    pInner += 4;
                }
            });

            bRes.UnlockBits(bmWork);
            bInner.UnlockBits(bmInner);
            bOut.UnlockBits(bmData);

            return bOut;
        }

        private unsafe Bitmap? GetInnerFGPic(Bitmap bWork, Bitmap bForeground)
        {
            int w = bWork.Width;
            int h = bWork.Height;
            Bitmap bRes = new Bitmap(w, h);

            BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmWork = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmFG = bForeground.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = bmData.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmData.Scan0;
                p += y * stride;
                byte* pWork = (byte*)bmWork.Scan0;
                pWork += y * stride;
                byte* pFG = (byte*)bmFG.Scan0;
                pFG += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pFG[3] > 0)
                    {
                        p[0] = pWork[0];
                        p[1] = pWork[1];
                        p[2] = pWork[2];
                        p[3] = pFG[3];
                    }

                    p += 4;
                    pWork += 4;
                    pFG += 4;
                }
            });

            bWork.UnlockBits(bmWork);
            bForeground.UnlockBits(bmFG);
            bRes.UnlockBits(bmData);

            return bRes;
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

        private Bitmap? ResampleUp(Bitmap bRes, double resPic, Bitmap? bOrig, bool dontFillPath, bool disposebOrig)
        {
            //take orig image,
            //get chains from result pic
            //"cut" (crop) orig image with chains as Mask

            if (bOrig != null)
            {
                Bitmap? bOut = new Bitmap(bOrig.Width, bOrig.Height);

                using (Bitmap bTmp = new Bitmap(bOrig.Width, bOrig.Height))
                {
                    using (Graphics gx = Graphics.FromImage(bTmp))
                        gx.DrawImage(bRes, 0, 0, bOut.Width, bOut.Height);

                    List<ChainCode> allChains = GetBoundary(bTmp);

                    using (TextureBrush tb = new TextureBrush(bOrig))
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

                    if (dontFillPath)
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

        private void _gc_ShowInfo(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => this.toolStripStatusLabel4.Text = e));
            else
                this.toolStripStatusLabel4.Text = e;
        }

        private void backgroundWorker6_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void backgroundWorker6_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap? bRes = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bRes, this.helplineRulerCtrl1, "Bmp");

                Bitmap? bC = new Bitmap(bRes);
                this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bRes);

            }

            this.btnPPGo.Text = "Go";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

            this.btnAlphaV.Text = "Go";

            this._pic_changed = true;

            if (this._gc != null)
            {
                this._gc.ShowInfo -= _gc_ShowInfo;
                this._gc.Dispose();
                this._gc = null;
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

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

        private Rectangle GetR(Bitmap bWork, int oW)
        {
            List<ChainCode> c = GetBoundary(bWork);
            using (GraphicsPath gP = new GraphicsPath())
            {
                for (int i = 0; i < c.Count; i++)
                {
                    gP.AddLines(c[i].Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                    gP.CloseFigure();
                }

                RectangleF rc = gP.GetBounds();
                rc.Inflate(oW, oW);

                return new Rectangle((int)Math.Floor(rc.X), (int)Math.Floor(rc.Y), (int)Math.Ceiling(rc.Width), (int)Math.Ceiling(rc.Height));
            }
        }

        private void cmbMethodMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.label4.Enabled = this.numTh.Enabled = (cmbMethodMode.SelectedIndex == 1 && !this.cbExpOutlProc.Checked);
            this.label48.Enabled = this.label47.Enabled = this.numNormalDist.Enabled = this.numColDistDist.Enabled =
                (cmbMethodMode.SelectedIndex == 0 && !this.cbExpOutlProc.Checked);
            //this.cbDesaturate.Enabled = (cmbMethodMode.SelectedIndex == 0 && !this.cbExpOutlProc.Checked);
        }

        private void cbRestoreDefects_CheckedChanged(object sender, EventArgs e)
        {
            this.label12.Enabled = this.label6.Enabled = this.label7.Enabled = this.label8.Enabled = (cbRestoreDefects.Checked && !this.cbExpOutlProc.Checked);
            this.numWMin.Enabled = this.numWMax.Enabled = this.numGamma2.Enabled = this.numWMax.Enabled = (cbRestoreDefects.Checked && !this.cbExpOutlProc.Checked);
            this.label10.Enabled = this.numWHFactor.Enabled = this.numOpacity.Enabled = (cbRestoreDefects.Checked && !this.cbExpOutlProc.Checked);
            this.btnPrecompAngles.Enabled = (cbRestoreDefects.Checked && !this.cbExpOutlProc.Checked);
        }

        private void cbShowAngles_CheckedChanged(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnPrecompAngles_Click(object sender, EventArgs e)
        {
            double wMin = (double)this.numWMin.Value;
            double wMax = (double)this.numWMax.Value;
            int whFactor = (int)this.numWHFactor.Value;

            PrecomuteConvexDefectsAngles(this.helplineRulerCtrl1.Bmp, wMax, whFactor, wMin);

            this.cbShowAngles.Checked = true;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void PrecomuteConvexDefectsAngles(Bitmap bPrevious, double wMax = 180.0, int whFactor = 4, double wMin = 0)
        {
            //Get all connected components
            List<ChainCode> c = GetBoundary(bPrevious);
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;

            c = c.OrderByDescending(a => a.Coord.Count).ToList();

            if (this._rectsList == null)
                this._rectsList = new List<List<Rectangle>>();
            this._rectsList.Clear();

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
                            dp.Add(Tuple.Create(j, distB[j], distA[j]));
                        }
                    }

                    List<Point> rectPoints = new List<Point>();

                    //Size of bitmap
                    int wh = (this._oW + this._iW) * whFactor + 1;
                    if ((wh & 0x01) != 1)
                        wh++;
                    int wh2 = wh / 2;

                    for (int j = 0; j < dp.Count; j++)
                    {
                        Point pt = pts[dp[j].Item1];
                        int sx = Math.Max(pt.X - wh2, 0);
                        int sy = Math.Max(pt.Y - wh2, 0);

                        rects.Add(new Rectangle(new Point(sx, sy), new Size(wh, wh)));
                    }
                }

                this._rectsList.Add(rects);
            }
            //#############################################################################
        }

        private void cbOnlyRestore_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control ct in this.groupBox4.Controls)
                ct.Enabled = !cbOnlyRestore.Checked;

            cbOnlyRestore.Enabled = true;

            if (!this.cbOnlyRestore.Checked)
            {
                DisableBoundControls(this.cbExpOutlProc.Checked);
                this.btnOnlyRestore.Enabled = false;
            }
            else
            {
                this.cbRestoreDefects.Enabled = this.label8.Enabled = true;
                this.label6.Enabled = this.label7.Enabled = this.label10.Enabled = this.label12.Enabled = true;
                this.numOpacity.Enabled = this.numWMin.Enabled = this.numWMax.Enabled = true;
                this.numGamma2.Enabled = this.numWHFactor.Enabled = true;
                this.btnOnlyRestore.Enabled = this.cbShowAngles.Enabled = this.btnPrecompAngles.Enabled = true;
            }
        }

        private void btnOnlyRestore_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker7.IsBusy)
            {
                this.backgroundWorker7.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnOnlyRestore.Text = "Cancel";
                this.btnOnlyRestore.Enabled = true;

                double gamma2 = (double)this.numGamma2.Value;
                float opacity = (float)this.numOpacity.Value;
                double wMin = (double)this.numWMin.Value;
                double wMax = (double)this.numWMax.Value;
                int whFactor = (int)this.numWHFactor.Value;

                this.backgroundWorker7.RunWorkerAsync(new object[] { this.helplineRulerCtrl1.Bmp, gamma2, opacity, wMin, wMax, whFactor });
            }
        }

        private void backgroundWorker7_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap bWork = new Bitmap((Bitmap)o[0]);
                double gamma = (double)o[1];
                float opacity = (float)o[2];
                double wMin = (double)o[3];
                double wMax = (double)o[4];
                int whFactor = (int)o[5];

                RevisitConvexDefects(this.helplineRulerCtrl1.Bmp, bWork, gamma, opacity, wMax, whFactor, wMin);

                e.Result = bWork;
            }
        }

        private void backgroundWorker7_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Bitmap? bmp = null;

            if (e.Result != null)
                bmp = (Bitmap)e.Result;

            if (bmp != null)
            {
                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                Bitmap? bC = new Bitmap(bmp);
                this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bmp);
            }

            this.btnOnlyRestore.Text = "restore";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this.cbExpOutlProc_CheckedChanged(this.cbExpOutlProc, new EventArgs());

            this._pic_changed = true;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            //if (this.Timer3.Enabled)
            //    this.Timer3.Stop();

            //this.Timer3.Start();

            this.backgroundWorker7.Dispose();
            this.backgroundWorker7 = new BackgroundWorker();
            this.backgroundWorker7.WorkerReportsProgress = true;
            this.backgroundWorker7.WorkerSupportsCancellation = true;
            this.backgroundWorker7.DoWork += backgroundWorker7_DoWork;
            //this.backgroundWorker7.ProgressChanged += backgroundWorker7_ProgressChanged;
            this.backgroundWorker7.RunWorkerCompleted += backgroundWorker7_RunWorkerCompleted;
        }

        private void btnResVals_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                double d = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true, (double)this.numMaxSize.Value);
                this.numBoundInner.Value = 5;
                this.numBoundOuter.Value = 5;
                int f2 = this.cbHalfSize.Checked ? 2 : 1;
                this.cbHalfSize.Checked = true;

                if (d > 1)
                {
                    this.numBoundInner.Value = (int)(5 * d * f2);
                    this.numBoundOuter.Value = (int)(5 * d * f2);
                }
            }
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

                this.SetBitmap(ref this._bmpOrig, ref bmp);
                this.pictureBox1.Image = this._bmpOrig;
                this.pictureBox1.Refresh();
            }
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            //bmpRef, bmpTrimap, bmpMatte, bmpWork, bmpOrig
            using frmPictures frm = new(_bmpRef, _bmpTrimap, _bmpMatte, _bmpWork, _bmpOrig);
            frm.ShowDialog();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            frmEdgePic frm4 = new frmEdgePic(this.pictureBox1.Image, this.helplineRulerCtrl1.Bmp.Size);
            frm4.Text = "Orig";
            frm4.ShowDialog();
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

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
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

        private void btnCreateTrimap_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker2.IsBusy)
            {
                this.backgroundWorker2.CancelAsync();
                return;
            }
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }
            if (this.backgroundWorker6.IsBusy)
            {
                this.backgroundWorker6.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                this.btnOK.Enabled = this.btnCancel.Enabled = btnCreateTrimap.Enabled = false;
                this.btnCreateTrimap.Refresh();

                int innerW = this._iW;
                int outerW = this._oW;
                bool redrawInner = this.cbRedrawInner.Checked;

                bool editTrimap = this.cbEditTrimap.Checked;

                double res = CheckWidthHeight(this._bmpOrig, true, (double)this.numMaxSize.Value);
                this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                Size? sz = new Size(this._bmpOrig.Width, this._bmpOrig.Height);

                if (res > 1)
                    sz = new Size((int)Math.Ceiling(this._bmpOrig.Width / res), (int)Math.Ceiling(this._bmpOrig.Height / res));

                if (sz != null && cbHalfSize.Checked)
                    sz = new Size(sz.Value.Width / 2, sz.Value.Height / 2);

                if (sz != null)
                {
                    double factor = ((this.cbHalfSize.Checked)) ? 2.0 : 1.0;
                    factor *= (res > 1) ? res : 1.0;

                    Bitmap? bTrimap = null;

                    if (this._bmpTrimap != null && this.cbUseExistingTrimap.Checked)
                    {
                        Size sTest = new((int)Math.Ceiling(this.helplineRulerCtrl1.Bmp.Width / factor),
                            (int)Math.Ceiling(this.helplineRulerCtrl1.Bmp.Height / factor));

                        if (sTest.Width != this._bmpTrimap.Width || sTest.Height != this._bmpTrimap.Height)
                        {
                            Bitmap? bTmp2 = new Bitmap(sTest.Width, sTest.Height);
                            using Graphics gx = Graphics.FromImage(bTmp2);
                            gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            gx.DrawImage(this._bmpTrimap, 0, 0, bTmp2.Width, bTmp2.Height);
                            this.SetBitmap(ref this._bmpTrimap, ref bTmp2);
                            if (bTmp2 != null)
                            {
                                Bitmap? bOld2 = bTrimap;
                                bTrimap = new Bitmap(bTmp2);
                                if (bOld2 != null)
                                    bOld2.Dispose();
                                bOld2 = null;
                            }
                        }
                        else
                            bTrimap = new Bitmap(this._bmpTrimap);
                    }
                    else
                    {
                        innerW = (int)Math.Max(Math.Ceiling(innerW / factor), 1);
                        outerW = (int)Math.Max(Math.Ceiling(outerW / factor), 1);

                        bTrimap = new Bitmap(sz.Value.Width, sz.Value.Height);

                        Bitmap? bW = new Bitmap(this.helplineRulerCtrl1.Bmp);

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
                                List<ChainCode> c = GetBoundary(bTrimapTmp);
                                ChainFinder cf = new ChainFinder();
                                cf.AllowNullCells = true;

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
                                //
                            }

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
                    }

                    if (bTrimap != null)
                    {
                        if (editTrimap)
                        {
                            using Bitmap bWork = new Bitmap(bTrimap.Width, bTrimap.Height);
                            using Graphics gx = Graphics.FromImage(bWork);
                            gx.DrawImage(this._bmpOrig, 0, 0, bWork.Width, bWork.Height);
                            using (frmEditTrimap frm = new frmEditTrimap(bTrimap, bWork, factor))
                            {
                                Bitmap? bmp = null;

                                if (frm.ShowDialog() == DialogResult.OK && frm.FBitmap != null)
                                {
                                    bmp = new Bitmap(frm.FBitmap);

                                    Bitmap? bOld2 = bTrimap;
                                    bTrimap = bmp;
                                    if (bOld2 != null)
                                    {
                                        bOld2.Dispose();
                                        bOld2 = null;
                                    }
                                }
                            }
                        }

                        this.SetBitmap(ref this._bmpTrimap, ref bTrimap);
                        if (this._bmpTrimap != null)
                        {
                            Image? iOld = this.pictureBox2.Image;
                            this.pictureBox2.Image = new Bitmap(this._bmpTrimap);
                            this.pictureBox2.Refresh();

                            if (iOld != null)
                            {
                                iOld.Dispose();
                                iOld = null;
                            }
                        }
                    }
                }

                this.btnOK.Enabled = this.btnCancel.Enabled = btnCreateTrimap.Enabled = true;
            }
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox2.Image != null)
            {
                frmEdgePic frm4 = new frmEdgePic(this.pictureBox2.Image, this.helplineRulerCtrl1.Bmp.Size);
                frm4.Text = "Trimap";
                frm4.ShowDialog();
            }
        }

        private void btnAlphaZAndGain_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker8.IsBusy)
            {
                this.backgroundWorker8.CancelAsync();
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

                this.backgroundWorker8.RunWorkerAsync(new object[] { b, alphaTh, redrawExcluded });
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

        private void backgroundWorker8_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = new Bitmap((Bitmap)o[0]);
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

        private void backgroundWorker8_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
                            {
                                SetTransp(bmp, this._excludedRegions[i], this._exclLocations[i]);
                                gx.DrawImage(this._excludedRegions[i], this._exclLocations[i]);
                            }
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

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.btnAlphaZAndGain.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();
            }

            this.backgroundWorker8.Dispose();
            this.backgroundWorker8 = new BackgroundWorker();
            this.backgroundWorker8.WorkerReportsProgress = true;
            this.backgroundWorker8.WorkerSupportsCancellation = true;
            this.backgroundWorker8.DoWork += backgroundWorker8_DoWork;
            //this.backgroundWorker8.ProgressChanged += backgroundWorker8_ProgressChanged;
            this.backgroundWorker8.RunWorkerCompleted += backgroundWorker8_RunWorkerCompleted;

        }

        private void cbForceSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (cbForceSerial.Checked)
                this.numSleep.Value = (decimal)0;
            else
                this.numSleep.Value = (decimal)10;
        }
    }
}
