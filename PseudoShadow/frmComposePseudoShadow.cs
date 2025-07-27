using Cache;
using ConvolutionLib;
using LUBitmapDesigner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudoShadow
{
    public partial class frmComposePseudoShadow : Form
    {
        private bool _pic_changed;
        private Bitmap? _bmpBU;
        private Tuple<int, int>[] _cacheMappings = new Tuple<int, int>[2];
        public Bitmap? FBitmap { get; private set; }

        private UndoOPCache? _undoOPCache = null;
        private UndoOPCache? _undoOPCache2 = null;

        private Bitmap? _bmpBGOrig;

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

        public string? AvoidAGrabCutCache { get; set; }

        private string? m_CachePathAddition;

        private static int[] CustomColors = new int[] { };
        private List<Bitmap>? _excludedRegions;
        private List<Point>? _exclLocations;

        public frmComposePseudoShadow(Bitmap? bUpperTmp, bool scf, string basePathAddition)
        {
            InitializeComponent();

            this.luBitmapDesignerCtrl1.ShadowMode = true;

            CachePathAddition = basePathAddition;

            Bitmap? bUpper = null;

            if (bUpperTmp != null)
            {
                bUpper = scf ? ScanForPic(bUpperTmp, 0) : new Bitmap(bUpperTmp);
                bUpperTmp.Dispose();
                bUpperTmp = null;

                if (bUpper != null && AvailMem.AvailMem.checkAvailRam(bUpper.Width * bUpper.Height * 16L))
                {
                    this._bmpBU = new Bitmap(bUpper);
                    this.luBitmapDesignerCtrl1.SetUpperImage(new Bitmap(bUpper), 0, 0);
                    this.luBitmapDesignerCtrl1.SetupBGImage();
                }
                else
                    MessageBox.Show("Not enough memory.");

                this.luBitmapDesignerCtrl1.ShapeChanged += LuBitmapDesignerCtrl1_ShapeChanged;
                this.picInfoCtrl1.ShapeChanged += PicInfoCtrl1_ShapeChanged;
                this.luBitmapDesignerCtrl1.ShapeRemoved += LuBitmapDesignerCtrl1_ShapeRemoved;
                this.luBitmapDesignerCtrl1.SPC += LuBitmapDesignerCtrl1_SPC;
                this.picInfoCtrl1.btnBGSettings.Click += BtnBGSettings_Click;

                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.MouseMove += DbPanel1_MouseMove;

                if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0)
                    this.LuBitmapDesignerCtrl1_ShapeChanged(this, this.luBitmapDesignerCtrl1.ShapeList[this.luBitmapDesignerCtrl1.ShapeList.Count - 1]);
            }
        }

        private void BtnBGSettings_Click(object? sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0 &&
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp != null && this._bmpBGOrig != null)
            {
                Bitmap? bOrig = new Bitmap(this._bmpBGOrig);

                frmBGSettings frm = new frmBGSettings(bOrig.Size,
                    new Size((int)this.luBitmapDesignerCtrl1.ShapeList[0].Bounds.Width,
                    (int)this.luBitmapDesignerCtrl1.ShapeList[0].Bounds.Height));
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Bitmap? bmp = new Bitmap((int)frm.numWidth.Value, (int)frm.numHeight.Value);
                    using Graphics gx = Graphics.FromImage(bmp);
                    gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gx.DrawImage(bOrig, 0, 0, bmp.Width, bmp.Height);

                    BitmapShape? b = this.luBitmapDesignerCtrl1.ShapeList[0];
                    BitmapShape bNew = new BitmapShape() { Bmp = bmp, Bounds = new RectangleF(0, 0, bmp.Width, bmp.Height), Rotation = 0, Zoom = this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Zoom };
                    this.luBitmapDesignerCtrl1.ShapeList[0] = bNew;
                    this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp = bNew.Bmp;

                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());
                }
            }
        }

        private void DbPanel1_MouseMove(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Zoom);
            int iy = (int)((e.Y - this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Zoom);

            //int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
            //int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

            if (ix >= 0 && ix < this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp.Height)
            {
                this.toolStripStatusLabel1.Text = "x: " + ix.ToString() + ", y: " + iy.ToString();
                Color c = this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                this.toolStripStatusLabel3.Text = c.ToString();
                this.toolStripStatusLabel2.BackColor = c;
            }
        }

        public Bitmap? ScanForPic(Bitmap bmp, int add)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData? bmData = null;

            try
            {
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int scanline = bmData.Stride;

                System.IntPtr Scan0 = bmData.Scan0;

                Point top = new Point(), left = new Point(), right = new Point(), bottom = new Point();
                bool complete = false;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if (p[3] != 0)
                            {
                                top = new Point(x, y);
                                complete = true;
                                break;
                            }

                            p += 4;
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int y = bmp.Height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < bmp.Width; x++)
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

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
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

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int x = bmp.Width - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < bmp.Height; y++)
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
                }

                bmp.UnlockBits(bmData);

                Rectangle rectangle = new Rectangle(left.X, top.Y, right.X - left.X, bottom.Y - top.Y);

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
            }

            return null;
        }

        private void LuBitmapDesignerCtrl1_SPC(object? sender, bool e)
        {
            this.btnUndo.Enabled = this.btnRedo.Enabled = e;

            this.btnAlphaZAndGain.Enabled = this.btnSetGamma.Enabled = e;
            this.btnFloodfill.Enabled = this.btnGaussian.Enabled = this.btnShear.Enabled = e;
            this.btnClone.Enabled = this.btnRemove.Enabled = this.btnLoadUpper.Enabled = e;
        }

        private void LuBitmapDesignerCtrl1_ShapeRemoved(object? sender, BitmapShape e)
        {
            this.btnRemove.PerformClick();
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmCompose");
            if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
            {
                _undoOPCache.Add(this.luBitmapDesignerCtrl1.ShapeList[1].Bmp);
                this._cacheMappings[0] = Tuple.Create(1, this.luBitmapDesignerCtrl1.ShapeList[1].ID);
            }
        }

        private void PicInfoCtrl1_ShapeChanged(object? sender, BitmapShape e)
        {
            this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();
            e.IsLocked = this.picInfoCtrl1.cbLock.Checked;
        }

        private void LuBitmapDesignerCtrl1_ShapeChanged(object? sender, BitmapShape e)
        {
            if (e != null)
                this.picInfoCtrl1.SetValues(e);
        }

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void cmbZoom_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.Visible && this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp != null && cmbZoom.SelectedItem != null)
            {
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.DontDoLayout = true;
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Enabled = false;
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Refresh();
                this.luBitmapDesignerCtrl1.SetZoom(cmbZoom.SelectedItem.ToString());
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Enabled = true;
                if (this.cmbZoom.SelectedIndex < 2)
                    this.luBitmapDesignerCtrl1.helplineRulerCtrl1.ZoomSetManually = true;

                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.DontDoLayout = false;
                this.luBitmapDesignerCtrl1.SetupBGImage();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = DrawToBmp();

                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _pic_changed = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                bmp?.Dispose();
                bmp = null;
            }
        }

        private Bitmap? DrawToBmp()
        {
            Bitmap? bOut = null;

            if (this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp != null)
            {
                bOut = new Bitmap(this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp.Width, this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp.Height);
                using (Graphics gx = Graphics.FromImage(bOut))
                    if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0)
                        gx.DrawImage(this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp, 0, 0);

                if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
                {
                    float zBU = this.luBitmapDesignerCtrl1.ShapeList[0].Zoom;

                    for (int j = 1; j < this.luBitmapDesignerCtrl1.ShapeList.Count; j++)
                    {
                        this.luBitmapDesignerCtrl1.ShapeList[j].Zoom = 1.0f;
                        this.luBitmapDesignerCtrl1.ShapeList[j].Draw(bOut);
                        this.luBitmapDesignerCtrl1.ShapeList[j].Zoom = zBU;
                    }
                }
            }

            return bOut;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp != null)
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

                        this.luBitmapDesignerCtrl1?.ShapeList?.ClearWithoutBG();
                        this.luBitmapDesignerCtrl1?.SetUpperImage(b1, 0, 0);

                        this._pic_changed = false;

                        this.luBitmapDesignerCtrl1?.SetZoom(this.cmbZoom?.SelectedItem?.ToString());
                        this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.MakeBitmap(this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp);
                        if (this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.dbPanel1 != null)
                            this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.Bmp.Width * this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.Bmp.Height * this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.Zoom));
                        if (this.luBitmapDesignerCtrl1 != null)
                            this.luBitmapDesignerCtrl1.SelectedShape = null;
                        this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.dbPanel1.Invalidate();

                        _undoOPCache?.Reset(true);
                        this.btnUndo.Enabled = this.btnRedo.Enabled = false;

                        this.luBitmapDesignerCtrl1?.SetupBGImage();
                    }
                    catch
                    {
                        if (b1 != null)
                            b1.Dispose();
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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void frmCompose_Load(object sender, EventArgs e)
        {
            foreach (string z in System.Enum.GetNames(typeof(MergeOperation)))
                this.picInfoCtrl1.cmbMergeOP.Items.Add(z.ToString());

            this.picInfoCtrl1.cmbMergeOP.SelectedIndex = 0;

            this.cmbZoom.SelectedIndex = 4;
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;
                using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    bmp = new Bitmap(img);

                if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0)
                {
                    BitmapShape? b = this.luBitmapDesignerCtrl1.ShapeList[0];
                    BitmapShape bNew = new BitmapShape() { Bmp = bmp, Bounds = new RectangleF(0, 0, bmp.Width, bmp.Height), Rotation = 0, Zoom = this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Zoom };
                    this.luBitmapDesignerCtrl1.ShapeList[0] = bNew;
                    this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp = bNew.Bmp;

                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());

                    this.btnCloneColors.Enabled = this.luBitmapDesignerCtrl1.ShapeList.Count > 1;

                    Bitmap bC = new Bitmap(bmp);
                    this.SetBitmap(ref this._bmpBGOrig, ref bC);
                }
            }
        }

        private void frmCompose_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0)
            {
                for (int i = this.luBitmapDesignerCtrl1.ShapeList.Count - 1; i >= 0; i--)
                    this.luBitmapDesignerCtrl1.ShapeList[i].Dispose();
            }

            if (this._undoOPCache != null)
                this._undoOPCache.Dispose();
            if (this._undoOPCache2 != null)
                this._undoOPCache2.Dispose();
            //bitmaps disposed in designer file

            this.luBitmapDesignerCtrl1.DisposeBGImage();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.FBitmap = this.DrawToBmp();
        }

        private void btnAlphaZAndGain_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                return;
            }

            if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
            {
                Bitmap? b = this.luBitmapDesignerCtrl1.GetUpperImage();
                if (b != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    this.SetControls(false);

                    this.btnAlphaZAndGain.Enabled = true;
                    this.btnAlphaZAndGain.Text = "Cancel";

                    int alphaTh = (int)this.numAlphaZAndGain.Value;

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

                    this.backgroundWorker1.RunWorkerAsync(new object[] { b, alphaTh, redrawExcluded });
                }
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

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
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

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
                    float x = 0f;
                    float y = 0f;

                    if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
                    {
                        x = this.luBitmapDesignerCtrl1.ShapeList[1].Bounds.X;
                        y = this.luBitmapDesignerCtrl1.ShapeList[1].Bounds.Y;
                    }

                    if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                        this.luBitmapDesignerCtrl1.SetSelectedImage(bmp, x, y);
                    //else
                    //    this.luBitmapDesignerCtrl1.SetUpperImage(bmp, x, y);

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());

                    //_undoOPCache?.Add(bmp);
                    if (this._undoOPCache != null && this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                    {
                        int id = this.luBitmapDesignerCtrl1.SelectedShape.ID;
                        if (this._cacheMappings[0].Item2 == id)
                            this._undoOPCache?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                        else
                            this._undoOPCache2?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                    }

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.btnAlphaZAndGain.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.luBitmapDesignerCtrl1.SetupBGImage();
                //this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.backgroundWorker1.Dispose();
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorker1.WorkerReportsProgress = true;
                this.backgroundWorker1.WorkerSupportsCancellation = true;
                this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                //this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            }
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
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;
                    }
                    foreach (Control ct in this.splitContainer2.Panel2.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;
                    }

                    this.luBitmapDesignerCtrl1.Enabled = this.luBitmapDesignerCtrl1.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.splitContainer1.Panel2.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;
                }
                foreach (Control ct in this.splitContainer2.Panel2.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;
                }

                this.luBitmapDesignerCtrl1.Enabled = this.luBitmapDesignerCtrl1.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
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

        private void backgroundWorker2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
                    float x = 0f;
                    float y = 0f;

                    if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
                    {
                        x = this.luBitmapDesignerCtrl1.ShapeList[1].Bounds.X;
                        y = this.luBitmapDesignerCtrl1.ShapeList[1].Bounds.Y;
                    }

                    if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                        this.luBitmapDesignerCtrl1.SetSelectedImage(bmp, x, y);
                    //else
                    //    this.luBitmapDesignerCtrl1.SetUpperImage(bmp, x, y);

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());

                    //_undoOPCache?.Add(bmp);
                    if (this._undoOPCache != null && this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                    {
                        int id = this.luBitmapDesignerCtrl1.SelectedShape.ID;
                        if (this._cacheMappings[0].Item2 == id)
                            this._undoOPCache?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                        else
                            this._undoOPCache2?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                    }

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.btnSetGamma.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.luBitmapDesignerCtrl1.SetupBGImage();

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.backgroundWorker2.Dispose();
                this.backgroundWorker2 = new BackgroundWorker();
                this.backgroundWorker2.WorkerReportsProgress = true;
                this.backgroundWorker2.WorkerSupportsCancellation = true;
                this.backgroundWorker2.DoWork += backgroundWorker2_DoWork;
                //this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
                this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap? bOut = null;

                if (this.luBitmapDesignerCtrl1.SelectedShape != null)
                {
                    if (this.luBitmapDesignerCtrl1.SelectedShape.ID == this._cacheMappings[0].Item2)
                        bOut = _undoOPCache.DoUndo();
                    else
                        bOut = _undoOPCache2?.DoUndo();
                }

                if (bOut != null)
                {
                    BitmapShape? b = this.luBitmapDesignerCtrl1?.SelectedShape;
                    if (b != null)
                        b.Bmp = bOut;

                    if (_undoOPCache.CurrentPosition < 1)
                    {
                        this.btnUndo.Enabled = false;
                        this._pic_changed = false;
                    }

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());

                    this.CheckRedoButton();

                    this.luBitmapDesignerCtrl1?.SetupBGImage();
                }
                else
                    MessageBox.Show("Error while undoing.");
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                //this._currentPosition++;
                //Tuple<int, int> f = this._undoList[this._currentPosition];
                //int pos = f.Item1;
                //string p = Path.Combine(this._undoOPCache.GetCachePath(), pos.ToString() + ".png");
                //Bitmap? bOut = null;
                //using Image img = Image.FromFile(p);
                //bOut = new Bitmap(img);
                //int id = f.Item2;

                Bitmap? bOut = null;

                if (this.luBitmapDesignerCtrl1.SelectedShape != null)
                {
                    if (this.luBitmapDesignerCtrl1.SelectedShape.ID == this._cacheMappings[0].Item2)
                        bOut = _undoOPCache.DoRedo();
                    else
                        bOut = _undoOPCache2?.DoRedo();
                }

                if (bOut != null)
                {
                    BitmapShape? b = this.luBitmapDesignerCtrl1?.SelectedShape;
                    if (b != null)
                        b.Bmp = bOut;

                    this._pic_changed = true;

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());

                    this.CheckRedoButton();
                    this.btnUndo.Enabled = true;

                    this.luBitmapDesignerCtrl1?.SetupBGImage();
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
        }

        private void btnSetGamma_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker2.IsBusy)
            {
                this.backgroundWorker2.CancelAsync();
                return;
            }

            if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
            {
                Bitmap? b1 = this.luBitmapDesignerCtrl1.GetUpperImage();
                if (b1 != null)
                {
                    Bitmap? b = new Bitmap(b1);
                    if (b != null)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this.SetControls(false);

                        this.btnSetGamma.Enabled = true;
                        this.btnSetGamma.Text = "Cancel";

                        double gamma = (double)this.numGamma.Value;

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

                        this.backgroundWorker2.RunWorkerAsync(new object[] { b, gamma, redrawExcluded });
                    }
                }
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.CustomColors = CustomColors;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.label3.BackColor = this.colorDialog1.Color;
                CustomColors = this.colorDialog1.CustomColors;
            }
        }

        private void btnFloodfill_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }

            if (this.luBitmapDesignerCtrl1.SelectedShape != null)
            {
                this.SetControls(false);
                this.btnFloodfill.Text = "Cancel";
                this.btnFloodfill.Enabled = true;
                this.backgroundWorker3.RunWorkerAsync();
            }
        }

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
            {
                Point pt = new Point((int)(this.luBitmapDesignerCtrl1.CurPt.X / (this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Width / this.luBitmapDesignerCtrl1.SelectedShape.Bmp.Width)),
                    (int)(this.luBitmapDesignerCtrl1.CurPt.Y / (this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Height / this.luBitmapDesignerCtrl1.SelectedShape.Bmp.Height)));
                FloodFillMethods.floodfill(this.luBitmapDesignerCtrl1.SelectedShape.Bmp,
                    pt.X, pt.Y, (int)this.numTolerance.Value,
                    this.luBitmapDesignerCtrl1.SelectedShape.Bmp.GetPixel(pt.X, pt.Y),
                    this.label3.BackColor, Int32.MaxValue, false, false, 1.0, false, false);
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            this.SetControls(true);
            this.btnFloodfill.Text = "Floodfill";
            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this._pic_changed = true;

            if (this._undoOPCache != null && this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
            {
                int id = this.luBitmapDesignerCtrl1.SelectedShape.ID;
                if (this._cacheMappings[0].Item2 == id)
                    this._undoOPCache?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                else
                    this._undoOPCache2?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
            }

            this.btnUndo.Enabled = true;
            this.CheckRedoButton();

            this.luBitmapDesignerCtrl1.SetupBGImage();

            this.backgroundWorker3.Dispose();
            this.backgroundWorker3 = new BackgroundWorker();
            this.backgroundWorker3.WorkerReportsProgress = true;
            this.backgroundWorker3.WorkerSupportsCancellation = true;
            this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;

            this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.ShapeList is ShadowShapeList)
            {
                ShadowShapeList sl = (ShadowShapeList)this.luBitmapDesignerCtrl1.ShapeList;
                sl.SwapUpperShapes();
                this.luBitmapDesignerCtrl1.SetupBGImage();
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.ShapeList != null &&
                this.luBitmapDesignerCtrl1.ShapeList.AddingAllowed(this.luBitmapDesignerCtrl1.ShadowMode) && this.luBitmapDesignerCtrl1.ShapeList != null &&
                this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
            {
                Bitmap? b = this.luBitmapDesignerCtrl1.GetUpperImage();
                if (b != null)
                {
                    this.luBitmapDesignerCtrl1.AddUpperImage(new Bitmap(b));
                    this.AddCache(b, this.luBitmapDesignerCtrl1.ShapeList[this.luBitmapDesignerCtrl1.ShapeList.Count - 1].ID);
                }

                this.luBitmapDesignerCtrl1.SetZoom(cmbZoom?.SelectedItem?.ToString());
                this.luBitmapDesignerCtrl1.SetupBGImage();
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.btnCloneColors.Enabled = this.luBitmapDesignerCtrl1.ShapeList.Count > 1;
            }

            if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.ShapeList != null &&
                !this.luBitmapDesignerCtrl1.ShapeList.AddingAllowed(this.luBitmapDesignerCtrl1.ShadowMode))
                this.btnClone.Enabled = this.btnLoadUpper.Enabled = false;
        }

        private void AddCache(Bitmap b, int id)
        {
            _undoOPCache2 = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmCompose2");
            _undoOPCache2.Add(b);
            if (this.luBitmapDesignerCtrl1.ShapeList != null)
                this._cacheMappings[1] = Tuple.Create(2, this.luBitmapDesignerCtrl1.ShapeList[1].ID);
        }

        private void btnGaussian_Click(object sender, EventArgs e)
        {
            if (!this.backgroundWorker4.IsBusy && this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
            {
                if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                {
                    Bitmap? b = this.luBitmapDesignerCtrl1.SelectedShape.Bmp;
                    if (b != null)
                    {
                        using frmGaussianBlur frm = new frmGaussianBlur(b);

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            this.SetControls(false);
                            this.btnGaussian.Text = "Cancel";
                            this.btnGaussian.Enabled = true;

                            this.backgroundWorker4.RunWorkerAsync(new object[] { frm.trackBar1.Value });
                        }
                    }
                }
            }
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && this.luBitmapDesignerCtrl1 != null)
            {
                bool b = false;
                object[] o = (object[])e.Argument;
                int blur = (int)o[0];
                if (blur < 3)
                    blur = 3;
                if ((blur & 1) != 0x01)
                    blur += 1;
                if (blur > 255)
                    blur = 255;

                if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 1)
                {
                    if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                    {
                        Bitmap? bmp = GetResizedBitmap(this.luBitmapDesignerCtrl1.SelectedShape.Bmp, blur);

                        if (bmp != null)
                        {
                            Convolution conv = new Convolution();
                            conv.CancelLoops = false;

                            conv.ProgressPlus += Conv_ProgressPlus;

                            if (this.backgroundWorker4.CancellationPending)
                                conv.CancelLoops = true;

                            b = Fipbmp.FastZGaussian_Blur_NxN(bmp, blur, blur > 3 ? 0.01 : 0.16, 255, true, true,
                                true, true, true, true, conv, true, false);

                            Bitmap? bOld = this.luBitmapDesignerCtrl1.SelectedShape.Bmp;
                            RectangleF? rc = new RectangleF(this.luBitmapDesignerCtrl1.SelectedShape.Bounds.X - blur,
                                  this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Y - blur,
                                  bmp.Width, bmp.Height);

                            this.luBitmapDesignerCtrl1.SelectedShape.Bounds = rc.Value;
                            this.luBitmapDesignerCtrl1.SelectedShape.Bmp = bmp;
                            if (bOld != null)
                                bOld.Dispose();
                            bOld = null;

                            conv.ProgressPlus -= Conv_ProgressPlus;
                        }
                    }
                    e.Result = b;
                }
            }
            else
                e.Result = false;
        }

        private Bitmap? GetResizedBitmap(Bitmap bmp, int blur)
        {
            Bitmap? bRes = new Bitmap(bmp.Width + blur * 2, bmp.Height + blur * 2);
            using Graphics gx = Graphics.FromImage(bRes);
            gx.DrawImage(bmp, blur, blur);

            return bRes;
        }

        private void Conv_ProgressPlus(object sender, ProgressEventArgs e)
        {
            this.backgroundWorker4.ReportProgress(Math.Min((int)((double)e.CurrentProgress / (double)e.ImgWidthHeight * 100), 100));
        }

        private void backgroundWorker4_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            this.SetControls(true);
            this.btnGaussian.Text = "Go";

            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this._pic_changed = true;

            if (this._undoOPCache != null && this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
            {
                int id = this.luBitmapDesignerCtrl1.SelectedShape.ID;
                if (this._cacheMappings[0].Item2 == id)
                    this._undoOPCache?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                else
                    this._undoOPCache2?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
            }

            this.btnUndo.Enabled = true;
            this.CheckRedoButton();

            this.luBitmapDesignerCtrl1.SetupBGImage();

            this.backgroundWorker4.Dispose();
            this.backgroundWorker4 = new BackgroundWorker();
            this.backgroundWorker4.WorkerReportsProgress = true;
            this.backgroundWorker4.WorkerSupportsCancellation = true;
            this.backgroundWorker4.DoWork += backgroundWorker4_DoWork;
            this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
            this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
            {
                Bitmap? b = this.luBitmapDesignerCtrl1.SelectedShape.Bmp;
                if (b != null)
                {
                    double x = Math.Abs(b.Width * (double)this.numericUpDown1.Value) + Math.Abs(b.Height * (double)this.numericUpDown3.Value);
                    double y = Math.Abs(b.Height * (double)this.numericUpDown2.Value) + Math.Abs(b.Width * (double)this.numericUpDown4.Value);

                    if (this.cbAuto.Checked)
                    {
                        if (this.numericUpDown2.Value < 0)
                            this.numericUpDown6.Value = (decimal)Math.Abs((double)this.numericUpDown2.Value) * b.Width;
                        else
                            this.numericUpDown6.Value = (decimal)0;

                        if (this.numericUpDown3.Value < 0)
                            this.numericUpDown5.Value = (decimal)Math.Abs((double)this.numericUpDown3.Value) * b.Height;
                        else
                            this.numericUpDown5.Value = (decimal)0;
                    }
                }
            }
        }

        private void btnShear_Click(object sender, EventArgs e)
        {
            this.btnShear.Enabled = false;
            this.btnShear.Refresh();
            if (this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
            {
                Bitmap? b = this.luBitmapDesignerCtrl1.SelectedShape.Bmp;
                if (b != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    SetControls(false);
                    this.Refresh();

                    float drawingWidth = this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Width;
                    float drawingHeight = this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Height;
                    float zoomX = drawingWidth / (float)this.luBitmapDesignerCtrl1.SelectedShape.Bmp.Width;
                    float zoomY = drawingHeight / (float)this.luBitmapDesignerCtrl1.SelectedShape.Bmp.Height;

                    Bitmap? bmp = null;
                    try
                    {
                        // neues Bild erstellen, dazu erst einmal die Größe berechnen
                        double x = Math.Abs(b.Width * (double)this.numericUpDown1.Value) + Math.Abs(b.Height * (double)this.numericUpDown3.Value);
                        double y = Math.Abs(b.Width * (double)this.numericUpDown2.Value) + Math.Abs(b.Height * (double)this.numericUpDown4.Value);

                        int w2 = (int)Math.Ceiling(x);
                        int h2 = (int)Math.Ceiling(y);

                        if (AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L))
                            bmp = new Bitmap(w2, h2);
                        else
                            throw new Exception();

                        // aus unseren numericUpDown-Controls die Werte einer System.Drawing.Drawing2D.Matrix zuweisen
                        Matrix mx = new Matrix((float)this.numericUpDown1.Value, (float)this.numericUpDown2.Value, (float)this.numericUpDown3.Value, (float)this.numericUpDown4.Value, (float)this.numericUpDown5.Value, (float)this.numericUpDown6.Value);

                        // Graphics-Object erstellen - und nach Verwendung disposen
                        using Graphics gx = Graphics.FromImage(bmp);
                        // ein paar Eigenschaften zur (zumindest optischen) Bildqualität
                        gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gx.SmoothingMode = SmoothingMode.AntiAlias;
                        // die Matrix dem Graphics-Object zuweisen 
                        gx.Transform = mx;
                        // das Bild herauszeichnen
                        gx.DrawImage(b, 0, 0);

                        Bitmap? bOld = this.luBitmapDesignerCtrl1.SelectedShape.Bmp;
                        RectangleF? rc = new RectangleF(this.luBitmapDesignerCtrl1.SelectedShape.Bounds.X,
                            this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Y,
                            w2 * zoomX, h2 * zoomY);

                        this.luBitmapDesignerCtrl1.SelectedShape.Bounds = rc.Value;
                        this.luBitmapDesignerCtrl1.SelectedShape.Bmp = bmp;

                        if (bOld != null)
                        {
                            bOld.Dispose();
                            bOld = null;
                        }

                        if (this._undoOPCache != null && this.luBitmapDesignerCtrl1.SelectedShape != null && this.luBitmapDesignerCtrl1.SelectedShape.Bmp != null)
                        {
                            int id = this.luBitmapDesignerCtrl1.SelectedShape.ID;
                            if (this._cacheMappings[0].Item2 == id)
                                this._undoOPCache?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                            else
                                this._undoOPCache2?.Add(this.luBitmapDesignerCtrl1.SelectedShape.Bmp);
                        }

                        this.btnUndo.Enabled = true;
                        this.CheckRedoButton();

                        // und Bescheidgeben, dass der Status nun "geändert" ist.
                        this._pic_changed = true;
                    }
                    catch
                    {
                        if (bmp != null)
                        {
                            bmp.Dispose();
                            bmp = null;
                        }
                        // z.B.: Wenn die ersten 4 Felder der Matrix aus 1, bzw. gleichen Werten stehen
                        MessageBox.Show("Error, or impossible Operation.", "Hello", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (this.luBitmapDesignerCtrl1.SelectedShape != null)
                        this.picInfoCtrl1.SetValues(this.luBitmapDesignerCtrl1.SelectedShape);

                    SetControls(true);
                    this.btnShear.Enabled = true;

                    this.luBitmapDesignerCtrl1.SetupBGImage();
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.SelectedShape != null)
            {
                this.luBitmapDesignerCtrl1.ShapeList?.Remove(this.luBitmapDesignerCtrl1.SelectedShape);
                this.luBitmapDesignerCtrl1.SelectedShape = null;
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();

                if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.ShapeList != null &&
                    this.luBitmapDesignerCtrl1.ShapeList.AddingAllowed(this.luBitmapDesignerCtrl1.ShadowMode))
                    this.btnClone.Enabled = this.btnLoadUpper.Enabled = true;

                this.luBitmapDesignerCtrl1?.SetupBGImage();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Space))
            {
                if (this.luBitmapDesignerCtrl1.SelectedShape != null)
                {
                    this.luBitmapDesignerCtrl1.SelectedShape.Bounds = new RectangleF(0, 0,
                        this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Width, this.luBitmapDesignerCtrl1.SelectedShape.Bounds.Height);

                    this.picInfoCtrl1.SetValues(this.luBitmapDesignerCtrl1.SelectedShape);
                    this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnLoadUpper_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.ShapeList != null && this.openFileDialog1.ShowDialog() == DialogResult.OK &&
                this.luBitmapDesignerCtrl1.ShapeList.AddingAllowed(this.luBitmapDesignerCtrl1.ShadowMode) && this.luBitmapDesignerCtrl1.ShapeList != null)
            {
                Bitmap? bTmp = null;
                using Image img = Image.FromFile(this.openFileDialog1.FileName);
                bTmp = new Bitmap(img);

                Bitmap? b = ScanForPic(bTmp, 0);
                bTmp.Dispose();
                bTmp = null;

                if (b != null)
                {
                    this.luBitmapDesignerCtrl1.AddUpperImage(new Bitmap(b));
                    this.AddCache(b, this.luBitmapDesignerCtrl1.ShapeList[this.luBitmapDesignerCtrl1.ShapeList.Count - 1].ID);
                }

                this.luBitmapDesignerCtrl1.SetZoom(cmbZoom?.SelectedItem?.ToString());
                this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.luBitmapDesignerCtrl1.SetupBGImage();
            }

            if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.ShapeList != null &&
                !this.luBitmapDesignerCtrl1.ShapeList.AddingAllowed(this.luBitmapDesignerCtrl1.ShadowMode))
                this.btnClone.Enabled = this.btnLoadUpper.Enabled = false;
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = DrawToBmp();

                if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0 && bmp != null)
                {
                    for (int i = this.luBitmapDesignerCtrl1.ShapeList.Count - 1; i > 0; i--) //only go to 1
                    {
                        //BitmapShape? b = this.luBitmapDesignerCtrl1.ShapeList[i];
                        this.luBitmapDesignerCtrl1.ShapeList.RemoveAt(i); //will also dispose it
                        //b.Dispose();
                        //b = null;
                    }

                    BitmapShape? bOld = this.luBitmapDesignerCtrl1.ShapeList[0];
                    BitmapShape bNew = new BitmapShape() { Bmp = bmp, Bounds = new RectangleF(0, 0, bmp.Width, bmp.Height), Rotation = 0, Zoom = this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Zoom };

                    if (!this.luBitmapDesignerCtrl1.helplineRulerCtrl1.IsDisposed)
                    {
                        this.luBitmapDesignerCtrl1.ShapeList[0] = bNew;
                        this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp = bNew.Bmp;
                        this.luBitmapDesignerCtrl1.helplineRulerCtrl1.MakeBitmap(this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp);
                        this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }

                    if (bOld != null)
                        bOld.Dispose();
                    bOld = null;

                    this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());
                    this.LuBitmapDesignerCtrl1_SPC(this, false);
                }

                _pic_changed = false;
            }
        }

        private void btnCloneColors_Click(object sender, EventArgs e)
        {
            if (this.luBitmapDesignerCtrl1 != null && this.luBitmapDesignerCtrl1.ShapeList != null &&
               this.luBitmapDesignerCtrl1.ShapeList.Count > 1 && this.CachePathAddition != null)
            {
                using frmCloneColors frm = new(this.luBitmapDesignerCtrl1.ShapeList, this.CachePathAddition);
                frm.SetupCache();

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.FBitmap != null)
                    {
                        Bitmap? result = new Bitmap(frm.FBitmap);
                        string? strID = frm.cmbDest?.SelectedItem?.ToString();
                        int j = -1;
                        if (Int32.TryParse(strID, out j))
                        {
                            BitmapShape? b = this.luBitmapDesignerCtrl1?.ShapeList?.GetShapeById(j);

                            if (b != null)
                            {
                                Bitmap? bOld = b.Bmp;
                                b.Bmp = result;

                                if (bOld != null && !bOld.Equals(b.Bmp) && !bOld.Equals(this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.Bmp))
                                {
                                    bOld.Dispose();
                                    bOld = null;
                                }

                                if (bOld != null && bOld.Equals(this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.Bmp))
                                {
                                    this.SetBitmap(this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp, result, this.luBitmapDesignerCtrl1.helplineRulerCtrl1, "Bmp");
                                    this.luBitmapDesignerCtrl1.helplineRulerCtrl1.MakeBitmap(this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp);
                                    this.luBitmapDesignerCtrl1.helplineRulerCtrl1.dbPanel1.Invalidate();
                                }

                                if (this._cacheMappings[0].Item2 == j)
                                    this._undoOPCache?.Add(b.Bmp);
                                else
                                    this._undoOPCache2?.Add(b.Bmp);

                                this.btnUndo.Enabled = true;
                                this._pic_changed = true;

                                this.luBitmapDesignerCtrl1?.SetZoom(cmbZoom?.SelectedItem?.ToString());
                                this.luBitmapDesignerCtrl1?.SetupBGImage();
                                this.luBitmapDesignerCtrl1?.helplineRulerCtrl1.dbPanel1.Invalidate();

                                this.btnCloneColors.Enabled = this.luBitmapDesignerCtrl1?.ShapeList.Count > 1;
                            }
                        }
                    }
                }
            }
        }

        private void btnFromCache_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.AvoidAGrabCutCache))
            {
                string? s = Path.GetDirectoryName(this.openFileDialog1.FileName);
                this.openFileDialog2.InitialDirectory = this.AvoidAGrabCutCache;
                if (this.openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    Bitmap? bmp = null;
                    using (Image img = Image.FromFile(this.openFileDialog2.FileName))
                        bmp = new Bitmap(img);

                    if (this.luBitmapDesignerCtrl1.ShapeList != null && this.luBitmapDesignerCtrl1.ShapeList.Count > 0)
                    {
                        BitmapShape? b = this.luBitmapDesignerCtrl1.ShapeList[0];
                        BitmapShape bNew = new BitmapShape() { Bmp = bmp, Bounds = new RectangleF(0, 0, bmp.Width, bmp.Height), Rotation = 0, Zoom = this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Zoom };
                        this.luBitmapDesignerCtrl1.ShapeList[0] = bNew;
                        this.luBitmapDesignerCtrl1.helplineRulerCtrl1.Bmp = bNew.Bmp;

                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }

                        this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());

                        this.btnCloneColors.Enabled = this.luBitmapDesignerCtrl1.ShapeList.Count > 1;

                        Bitmap bC = new Bitmap(bmp);
                        this.SetBitmap(ref this._bmpBGOrig, ref bC);
                    }
                }
                this.openFileDialog1.InitialDirectory = s;
            }
        }
    }
}
