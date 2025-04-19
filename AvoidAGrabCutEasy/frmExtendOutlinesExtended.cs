using AvoidAGrabCutEasy.ProcOutline;
using Cache;
using ChainCodeFinder;
using GetAlphaMatte;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmExtendOutlinesExtended : Form
    {
        private bool _dontDoZoom;
        private bool _pic_changed;
        private bool _dontAskOnClosing;
        private Bitmap? _bmpBU;
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

        private object _lockObject = new object();

        private Bitmap? _chainsBmp;
        private GraphicsPath? _hChain;
        private int _ix;
        private int _iy;
        private bool _tracking;
        private List<ChainCode>? _allChains;

        public Bitmap FBitmap
        {
            get
            {
                return this.helplineRulerCtrl1.Bmp;
            }
        }

        public event EventHandler<string>? BoundaryError;

        public frmExtendOutlinesExtended(Bitmap bmp, string? basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

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
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                //this._eX2 = e.X;
                //this._eY2 = e.Y;

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
                }
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (this._tracking && e.Button == MouseButtons.Left)
            {

            }

            this._tracking = false;
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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void frmProcOutline_FormClosing(object sender, FormClosingEventArgs e)
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
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();

                if (this._chainsBmp != null)
                    this._chainsBmp.Dispose();

                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();

                if (this._hChain != null)
                    this._hChain.Dispose();
                this._hChain = null;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
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

                    if (this.IsDisposed == false && this.Visible && this.helplineRulerCtrl1.Bmp != null)
                    {
                        if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                        {
                            if (this.backgroundWorker2.IsBusy)
                                this.backgroundWorker2.CancelAsync();

                            SetControls(false);

                            Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            int mOpacity = (int)0; //this.numChainTolerance.Value;

                            this.backgroundWorker2.RunWorkerAsync(new object[] { bmp, mOpacity });
                        }
                    }
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

                    if (this.IsDisposed == false && this.Visible && this.helplineRulerCtrl1.Bmp != null)
                    {
                        if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                        {
                            if (this.backgroundWorker2.IsBusy)
                                this.backgroundWorker2.CancelAsync();

                            SetControls(false);

                            Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            int mOpacity = (int)0; //this.numChainTolerance.Value;

                            this.backgroundWorker2.RunWorkerAsync(new object[] { bmp, mOpacity });
                        }
                    }
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
        }

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
                this.checkedListBox1.SetItemChecked(i, true);

            checkedListBox1_SelectedIndexChanged(this.checkedListBox1, new EventArgs());
        }

        private void btnSelNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
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

        private void btnClearPaths_Click(object sender, EventArgs e)
        {
            ClearExistingPaths();
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
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

        private void frmExtendOutlinesExtended_Load(object sender, EventArgs e)
        {
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());

            if (this.IsDisposed == false && this.Visible && this.helplineRulerCtrl1.Bmp != null)
            {
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                {
                    if (this.backgroundWorker2.IsBusy)
                        this.backgroundWorker2.CancelAsync();

                    SetControls(false);

                    Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                    int mOpacity = (int)0; //this.numChainTolerance.Value;

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bmp, mOpacity });
                }
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmChainCode");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
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
                        this._allChains = l;

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

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer1.Panel2.Controls)
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
                foreach (Control ct in this.splitContainer1.Panel2.Controls)
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

        private void cbSelSingleClick_CheckedChanged(object sender, EventArgs e)
        {
            this.checkedListBox1.CheckOnClick = this.cbSelSingleClick.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null && this.checkedListBox1.CheckedItems.Count > 0 && !this.backgroundWorker1.IsBusy)
            {
                this.SetControls(false);
                this.button1.Enabled = true;
                this.button1.Text = "Cancel";

                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                List<int> l = new List<int>();
                for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                    l.Add(this.checkedListBox1.CheckedIndices[i]);

                Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);

                int r = (int)this.numJRem1.Value;
                int ext = (int)this.numJRem2.Value;
                bool keepRightAngles = this.cbKeepRightAngles.Checked;

                this.backgroundWorker1.RunWorkerAsync(new object[] { bWork, l, r, ext, keepRightAngles });
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap bWork = (Bitmap)o[0];
                List<int> l = (List<int>)o[1];
                int r = (int)o[2];
                int ext = (int)o[3];
                bool keepRightAngles = (bool)o[4];

                double d = r + ext;
                double step = 100.0 / d;

                if (bWork != null)
                {
                    ChainFinder cf = new ChainFinder();
                    cf.AllowNullCells = true;

                    List<ChainCode>? fList = new List<ChainCode>();

                    for (int i = 0; i < this._allChains?.Count; i++)
                    {
                        if (l.Contains(i))
                            fList.Add(this._allChains[i]);
                    }

                    if (r > 0 && fList != null && fList.Count > 0)
                        for (int j = 0; j < r; j++)
                        {
                            this.backgroundWorker1.ReportProgress((int)(j * step));
                            if (this.backgroundWorker1.CancellationPending)
                                break;

                            cf.RemoveOutline(bWork, fList);
                            if (keepRightAngles)
                                cf.DoCornersR(bWork, fList);
                            List<ChainCode>? c = GetBoundary(bWork, 0, true);
                            c = c?.OrderByDescending(a => a.Chain.Count).ToList();

                            if (c != null)
                                foreach (ChainCode cc in c)
                                    UpdateFListRem(cc, fList);
                        }

                    if (ext > 0 && fList != null && fList.Count > 0)
                        for (int j = 0; j < ext; j++)
                        {
                            this.backgroundWorker1.ReportProgress((int)((j + r) * step));
                            if (this.backgroundWorker1.CancellationPending)
                                break;

                            cf.ExtendOutline(bWork, fList);
                            if (keepRightAngles)
                                cf.DoCornersE(bWork, fList);
                            List<ChainCode>? c = GetBoundary(bWork, 0, true);
                            c = c?.OrderByDescending(a => a.Chain.Count).ToList();

                            if (c != null)
                                foreach (ChainCode cc in c)
                                    UpdateFListExt(cc, fList);
                        }

                    e.Result = bWork;
                }
            }
        }

        //private unsafe void DoCornersR(Bitmap bWork, List<ChainCode> fList)
        //{
        //    int w = bWork.Width;
        //    int h = bWork.Height;

        //    BitmapData bmD = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    int stride = bmD.Stride;

        //    byte* p = (byte*)bmD.Scan0;

        //    for (int j = 0; j < fList.Count; j++)
        //    {
        //        if (ChainFinder.IsInnerOutline(fList[j]))
        //        {
        //            List<int> l = fList[j].Chain;
        //            int x = -1;
        //            int y = -1;

        //            if (l[0] == 1 && l[l.Count - 1] == 0)
        //            {
        //                x = fList[j].Coord[0].X;
        //                y = fList[j].Coord[0].Y - 1;

        //                p[x * 4 + y * stride + 3] = 0;
        //            }

        //            for (int i = 1; i < l.Count; i++)
        //            {
        //                if (l[i] == 0 && l[i - 1] == 3)
        //                {
        //                    x = fList[j].Coord[i].X - 1;
        //                    y = fList[j].Coord[i].Y;

        //                    p[x * 4 + y * stride + 3] = 0;
        //                }

        //                if (l[i] == 1 && l[i - 1] == 0)
        //                {
        //                    x = fList[j].Coord[i].X;
        //                    y = fList[j].Coord[i].Y - 1;

        //                    p[x * 4 + y * stride + 3] = 0;
        //                }

        //                if (l[i] == 2 && l[i - 1] == 1)
        //                {
        //                    x = fList[j].Coord[i].X + 1;
        //                    y = fList[j].Coord[i].Y;

        //                    p[x * 4 + y * stride + 3] = 0;
        //                }

        //                if (l[i] == 3 && l[i - 1] == 2)
        //                {
        //                    x = fList[j].Coord[i].X;
        //                    y = fList[j].Coord[i].Y + 1;

        //                    p[x * 4 + y * stride + 3] = 0;
        //                }
        //            }
        //        }
        //    }

        //    bWork.UnlockBits(bmD);
        //}

        //private unsafe void DoCornersE(Bitmap bWork, List<ChainCode> fList)
        //{
        //    int w = bWork.Width;
        //    int h = bWork.Height;

        //    BitmapData bmD = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    int stride = bmD.Stride;

        //    byte* p = (byte*)bmD.Scan0;

        //    for (int j = 0; j < fList.Count; j++)
        //    {
        //        if (!ChainFinder.IsInnerOutline(fList[j]))
        //        {
        //            List<int> l = fList[j].Chain;
        //            int x = -1;
        //            int y = -1;

        //            if (l[0] == 1 && l[l.Count - 1] == 2)
        //            {
        //                x = fList[j].Coord[0].X - 1;
        //                y = fList[j].Coord[0].Y - 1;

        //                p[x * 4 + y * stride + 3] = 255;
        //            }
        //        }
        //    }

        //    bWork.UnlockBits(bmD);
        //}

        private void UpdateFListRem(ChainCode cNew, List<ChainCode> fList)
        {
            for (int i = 0; i < fList.Count; i++)
            {
                bool isInnerOutlineOld = ChainFinder.IsInnerOutline(fList[i]);
                bool isInnerOutlineNew = ChainFinder.IsInnerOutline(cNew);
                if (isInnerOutlineOld == isInnerOutlineNew)
                {
                    ChainCode f = fList[i];
                    if (CompareChainsRem(cNew, f, isInnerOutlineOld))
                    {
                        fList[i] = cNew;
                        break;
                    }
                }
            }
        }

        private bool CompareChainsRem(ChainCode cNew, ChainCode cOld, bool isInnerOutline)
        {
            bool found = true;
            using GraphicsPath gP = new GraphicsPath();
            if (isInnerOutline)
            {
                gP.AddLines(cNew.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());

                foreach (Point pt in cOld.Coord)
                    if (!gP.IsVisible(pt))
                    {
                        found = false;
                        break;
                    }
            }
            else
            {
                gP.AddLines(cOld.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());

                foreach (Point pt in cNew.Coord)
                    if (!gP.IsVisible(pt))
                    {
                        found = false;
                        break;
                    }
            }

            return found;
        }

        private void UpdateFListExt(ChainCode cNew, List<ChainCode> fList)
        {
            for (int i = 0; i < fList.Count; i++)
            {
                bool isInnerOutlineOld = ChainFinder.IsInnerOutline(fList[i]);
                bool isInnerOutlineNew = ChainFinder.IsInnerOutline(cNew);
                if (isInnerOutlineOld == isInnerOutlineNew)
                {
                    ChainCode f = fList[i];
                    if (CompareChainsExt(cNew, f, isInnerOutlineOld))
                    {
                        fList[i] = cNew;
                        break;
                    }
                }
            }
        }

        private bool CompareChainsExt(ChainCode cNew, ChainCode cOld, bool isInnerOutline)
        {
            bool found = true;
            using GraphicsPath gP = new GraphicsPath();
            if (!isInnerOutline)
            {
                gP.AddLines(cNew.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());

                foreach (Point pt in cOld.Coord)
                    if (!gP.IsVisible(pt))
                    {
                        found = false;
                        break;
                    }
            }
            else
            {
                gP.AddLines(cOld.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());

                foreach (Point pt in cNew.Coord)
                    if (!gP.IsVisible(pt))
                    {
                        found = false;
                        break;
                    }
            }

            return found;
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap b = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this._pic_changed = true;
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                {
                    if (this.backgroundWorker2.IsBusy)
                        this.backgroundWorker2.CancelAsync();

                    SetControls(false);

                    Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                    int mOpacity = (int)0; //this.numChainTolerance.Value;

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bmp, mOpacity });
                }

                this.SetControls(true);
                this.button1.Text = "Go";

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
        }

        private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (!this.IsDisposed && this.Visible)
                this.toolStripProgressBar1.Value = Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum);
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            this.Timer3.Stop();
            this.toolStripProgressBar1.Value = 0;
            this.toolStripProgressBar1.Visible = false;
        }
    }
}
