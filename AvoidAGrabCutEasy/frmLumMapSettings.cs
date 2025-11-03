using AvoidAGrabCutEasy.ProcOutline;
using Cache;
using ChainCodeFinder;
using ConvolutionLib;
using SegmentsListLib;
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

namespace AvoidAGrabCutEasy
{
    public partial class frmLumMapSettings : Form
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

        private bool _pic_changed = false;
        private bool _dontDoZoom;
        private int _ix;
        private int _iy;
        private Bitmap? _bmpBU;
        private Bitmap? _baseImg;
        private UndoList? _bmpInfos;

        public frmLumMapSettings(Bitmap bmp, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            //HelperFunctions.HelperFunctions.SetFormSizeBig(this);
            this.CenterToScreen();

            // Me.helplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = (Bitmap)bmp.Clone();
                _bmpBU = (Bitmap)bmp.Clone();
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
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            this.helplineRulerCtrl1.dbPanel1.Capture = true;
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

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

                    toolStripStatusLabel1.Text = "x: " + _ix.ToString() + ", y: " + _iy.ToString() + " - " + " ARGB: " + c.A.ToString() + ";" + c.R.ToString() + ";" + c.G.ToString() + ";" + c.B.ToString();

                    this.toolStripStatusLabel2.BackColor = c;
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            this.helplineRulerCtrl1.dbPanel1.Capture = false;
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {

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
                                        b1 = (Bitmap)img.Clone();
                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                        b2 = (Bitmap)img.Clone();
                                        this.SetBitmap(ref this._bmpBU, ref b2);

                                        if (this._baseImg != null)
                                        {
                                            Image? iOld = this.pictureBox1.Image;
                                            this.pictureBox1.Image = new Bitmap(this._baseImg);
                                            this.pictureBox1.Refresh();

                                            if (iOld != null)
                                            {
                                                iOld.Dispose();
                                                iOld = null;
                                            }
                                        }
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

                                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                                this.Text = files[0] + " - frmQuickExtract";
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

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmLumMapSet");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            this.numThMultiplier.Enabled = this.label6.Enabled = !cbAuto.Checked;
        }

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoUndo();
                bool replacePBImage = false;

                if (this._bmpInfos?.CurrentPosition - 1 >= 0 && this._bmpInfos?[this._bmpInfos.CurrentPosition - 1].CachePosition == this._undoOPCache.CurrentPosition + 1) //this._undoOPCache.CurrentPosition already decremented
                {
                    this._bmpInfos.Undo();
                    replacePBImage = true;
                }

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

                    this.btnUndo.Enabled = this._undoOPCache.CurrentPosition > 1;

                    if (replacePBImage && this._bmpInfos != null && this._bmpInfos.CurrentPosition - 1 >= 0 && this._bmpInfos[this._bmpInfos.CurrentPosition - 1].Bmp != null)
                    {
                        Image? iOld = this.pictureBox1.Image;
                        BitmapInfo bi = this._bmpInfos[this._bmpInfos.CurrentPosition - 1];
                        if (bi.Bmp != null)
                            this.pictureBox1.Image = new Bitmap(bi.Bmp);
                        this.pictureBox1.Refresh();

                        if (iOld != null)
                        {
                            iOld.Dispose();
                            iOld = null;
                        }
                    }
                    else if (replacePBImage && this._bmpInfos != null && this._bmpInfos.CurrentPosition - 1 < 0 && this._baseImg != null)
                    {
                        Image? iOld = this.pictureBox1.Image;
                        this.pictureBox1.Image = new Bitmap(this._baseImg);
                        this.pictureBox1.Refresh();

                        if (iOld != null)
                        {
                            iOld.Dispose();
                            iOld = null;
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
                bool replacePBImage = false;

                if (this._bmpInfos?.CurrentPosition < this._bmpInfos?.Count && this._bmpInfos?[this._bmpInfos.CurrentPosition].CachePosition == this._undoOPCache.CurrentPosition) //this._undoOPCache.CurrentPosition already decremented
                {
                    this._bmpInfos.Redo();
                    replacePBImage = true;
                }

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = true;

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.CheckRedoButton();

                    this.btnUndo.Enabled = true;

                    if (replacePBImage && this._bmpInfos != null && this._bmpInfos.CurrentPosition <= this._bmpInfos.Count && this._bmpInfos[this._bmpInfos.CurrentPosition - 1].Bmp != null)
                    {
                        Image? iOld = this.pictureBox1.Image;

                        BitmapInfo bi = this._bmpInfos[this._bmpInfos.CurrentPosition - 1];
                        if (bi.Bmp != null)
                            this.pictureBox1.Image = new Bitmap(bi.Bmp);
                        this.pictureBox1.Refresh();

                        if (iOld != null)
                        {
                            iOld.Dispose();
                            iOld = null;
                        }
                    }
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
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

                            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                            this.helplineRulerCtrl1.dbPanel1.Invalidate();

                            _undoOPCache?.Reset(false);

                            if (this._baseImg != null)
                            {
                                Image? iOld = this.pictureBox1.Image;
                                this.pictureBox1.Image = new Bitmap(this._baseImg);
                                this.pictureBox1.Refresh();

                                if (iOld != null)
                                {
                                    iOld.Dispose();
                                    iOld = null;
                                }
                            }
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

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("");
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
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

        private void rbApp_CheckedChanged(object sender, EventArgs e)
        {
            if (rbApp.Checked)
            {
                this.groupBox1.Enabled = true;
                this.groupBox2.Enabled = this.groupBox3.Enabled = this.groupBox4.Enabled = false;
            }
            else
            {
                this.groupBox1.Enabled = false;
                this.groupBox2.Enabled = this.groupBox3.Enabled = this.groupBox4.Enabled = true;
            }
        }

        private void frmLumMapSettings_Load(object sender, EventArgs e)
        {
            this.rbApp_CheckedChanged(this.rbApp, new EventArgs());
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbZoom.SelectedIndex = 4;

            CreateBasePic();
        }

        private void CreateBasePic()
        {
            Bitmap bmp = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                using (Pen pen = new Pen(Color.Black, 2))
                    graphics.DrawLine(pen, new Point(0, bmp.Height), new Point(bmp.Width, 0));
            }

            this._baseImg = bmp;
            this.pictureBox1.Image = new Bitmap(this._baseImg);
            this.pictureBox1.Refresh();
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

        private void btnColors_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                byte[] rgb = new byte[256];
                List<Point> p = new List<Point>();
                p.Add(new Point(0, 0));
                p.Add(new Point((int)this.numValSrc.Value, (int)this.numValDst.Value));
                if (this.cbSecondColor.Checked)
                    p.Add(new Point((int)this.numValSrc2.Value, (int)this.numValDst2.Value));
                p.Add(new Point(255, 255));

                CurveSegment cuSgmt = new CurveSegment();
                List<BezierSegment> bz = cuSgmt.CalcBezierSegments(p.ToArray(), 0.5f);
                List<PointF> pts = cuSgmt.GetAllPoints(bz, 256, 0, 255);
                cuSgmt.MapPoints(pts, rgb);

                ColorCurves.fipbmp.GradColors(this.helplineRulerCtrl1.Bmp, rgb, rgb, rgb);

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.btnUndo.Enabled = true;
                CheckRedoButton();
                this._pic_changed = true;

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.CalculateZoom();
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                DisplayGradPic(p);
            }
        }

        private void DisplayGradPic(List<Point> p)
        {
            Bitmap bmp = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
            using (Graphics gx = Graphics.FromImage(bmp))
            {
                gx.SmoothingMode = SmoothingMode.AntiAlias;

                double factor = this.pictureBox1.ClientSize.Width / 255.0;

                List<PointF> pNew = new List<PointF>();
                for (int i = 0; i < p.Count; i++)
                    pNew.Add(new PointF((float)(p[i].X * factor), (float)(p[i].Y * factor)));
                pNew.Reverse();

                using (Pen pen = new Pen(Color.Black, 2f))
                {
                    gx.ScaleTransform(1f, -1f);
                    gx.TranslateTransform(0, bmp.Height, MatrixOrder.Append);
                    gx.DrawCurve(pen, pNew.ToArray());
                }
            }

            Image? iOld = this.pictureBox1.Image;
            this.pictureBox1.Image = bmp;
            this.pictureBox1.Refresh();

            if (iOld != null)
            {
                iOld.Dispose();
                iOld = null;
            }

            if (this._bmpInfos == null)
                this._bmpInfos = new UndoList();

            if (this._undoOPCache != null)
            {
                BitmapInfo bInfo = new BitmapInfo(new Bitmap(bmp), this._undoOPCache.CurrentPosition);
                if (this._bmpInfos.CurrentPosition < this._bmpInfos.Count && (this._bmpInfos.Count - this._bmpInfos.CurrentPosition) < this._bmpInfos.Count)
                    this._bmpInfos.RemoveRange(this._bmpInfos.CurrentPosition, this._bmpInfos.Count - this._bmpInfos.CurrentPosition);

                this._bmpInfos.Add(bInfo);
            }
        }

        private void btnBlur_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }

            if (!this.backgroundWorker3.IsBusy)
            {
                int krnl = (int)this.numKernel.Value;
                int maxVal = (int)this.numDistWeight.Value;

                SetControls(false);

                this.btnBlur.Text = "Cancel";
                this.btnBlur.Enabled = true;

                if (this.helplineRulerCtrl1.Bmp != null)
                {
                    Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                    this.backgroundWorker3.RunWorkerAsync(new object[] { bmp, krnl, maxVal });
                }
            }
        }

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = (Bitmap)o[0];
                int krnl = (int)o[1];
                int maxVal = (int)o[2];

                ConvolutionLib.Convolution conv = new ConvolutionLib.Convolution();
                conv.ProgressPlus += Conv_ProgressPlus;
                conv.CancelLoops = false;

                InvGaussGradOp igg = new InvGaussGradOp();
                igg.BGW = this.backgroundWorker3;

                igg.FastZGaussian_Blur_NxN_SigmaAsDistance(bmp, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);
                conv.ProgressPlus -= Conv_ProgressPlus;

                e.Result = bmp;
            }
        }

        private void Conv_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            this.backgroundWorker3.ReportProgress(Math.Min((int)(((double)e.CurrentProgress / (double)e.ImgWidthHeight) * 100), 100));
        }

        private void backgroundWorker3_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (!this.IsDisposed && this.Visible)
                {
                    Bitmap? bmpNew = null;

                    try
                    {
                        bmpNew = (Bitmap)e.Result;

                        if (bmpNew != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmpNew, this.helplineRulerCtrl1, "Bmp");
                            _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                            this.btnUndo.Enabled = true;
                            CheckRedoButton();
                            this._pic_changed = true;
                        }
                    }
                    catch
                    {
                    }

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.CalculateZoom();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    SetControls(true);

                    this.Cursor = Cursors.Default;

                    if (this.Timer3.Enabled)
                        this.Timer3.Stop();
                    this.toolStripProgressBar1.Value = 100;
                    this.Timer3.Start();

                    this.toolStripProgressBar1.Visible = false;
                    this.btnBlur.Text = "Blur";

                    // create a new bgw, since it reported "complete"
                    this.backgroundWorker3.Dispose();
                    this.backgroundWorker3 = new BackgroundWorker();
                    this.backgroundWorker3.WorkerReportsProgress = true;
                    this.backgroundWorker3.WorkerSupportsCancellation = true;
                    this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
                    this.backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
                    this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
                }
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

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer1.Panel1.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;

                        if (ct is GroupBox)
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
                foreach (Control ct in this.splitContainer1.Panel1.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;

                    if (ct is GroupBox)
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

        private void frmLumMapSettings_FormClosing(object sender, FormClosingEventArgs e)
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
                if (this.backgroundWorker3.IsBusy)
                    this.backgroundWorker3.CancelAsync();

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                this._bmpBU = null;
                if (this._baseImg != null)
                    this._baseImg.Dispose();

                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();

                if (this._bmpInfos != null)
                {
                    for (int i = this._bmpInfos.Count - 1; i >= 0; i--)
                        this._bmpInfos[i].Dispose();

                    this._bmpInfos.Clear();
                }
            }
        }

        private void btnInvGaussGrad_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }

            if (!this.backgroundWorker4.IsBusy && this.helplineRulerCtrl1.Bmp != null)
            {
                SetControls(false);

                this.btnInvGaussGrad.Text = "Cancel";
                this.btnInvGaussGrad.Enabled = true;

                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                int kernelLength = (int)numIGGKernel.Value;
                double cornerWeight = 0.01;
                int sigma = 255;
                double steepness = 1E-12;
                int radius = 340;
                double alpha = (double)this.numIGGAlpha.Value * 255.0;
                GradientMode gradientMode = GradientMode.Scharr16;
                double divisor = (double)this.numIGGDivisor.Value;
                bool grayscale = false;
                bool stretchValues = true;
                int threshold = 127;
                bool postBlur = this.cbPostBlur.Checked;
                int pBKrnl = (int)this.numPostBlurKrrnl.Value;
                bool invGaussGrad = this.rbIGG.Checked;
                int tolerance = (int)this.numTolerance.Value;
                bool procInner = this.cbProcInner.Checked;
                bool setInnerTransp = this.cbSetInnerTransp.Checked;
                bool useChainCode = this.cbUseChainCode.Checked;

                object[] o = { kernelLength, cornerWeight, sigma, steepness,
                               radius, alpha, gradientMode, divisor, grayscale, stretchValues,
                               threshold, postBlur, pBKrnl, invGaussGrad, tolerance, procInner,
                               setInnerTransp, useChainCode };

                this.backgroundWorker4.RunWorkerAsync(o);
            }
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && this.helplineRulerCtrl1.Bmp != null)
            {
                using (Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp))
                {
                    object[] o = (object[])e.Argument;

                    int kernelLength = (int)o[0];
                    double cornerWeight = (double)o[1];
                    int sigma = (int)o[2];
                    double steepness = (double)o[3];
                    int radius = (int)o[4];
                    double alpha = (double)o[5];
                    GradientMode gradientMode = (GradientMode)o[6];
                    double divisor = (double)o[7];
                    bool grayscale = (bool)o[8];
                    bool stretchValues = (bool)o[9];
                    int threshold = (int)o[10];
                    bool postBlur = (bool)o[11];
                    int pBKrnl = (int)o[12];
                    bool invGaussGrad = (bool)o[13];
                    int tolerance = (int)o[14];
                    bool procInner = (bool)o[15];
                    bool setInnerTransp = (bool)o[16];
                    bool useChainCode = (bool)o[17];

                    Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (r.Width > 0 && r.Height > 0)
                        {
                            InvGaussGradOp igg = new InvGaussGradOp();
                            igg.BGW = this.backgroundWorker4;

                            Bitmap? iG = null;

                            if (invGaussGrad)
                            {
                                iG = igg.Inv_InvGaussGrad(bmp, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                                        sigma, steepness, radius, stretchValues, threshold);
                            }
                            else
                            {
                                Grayscale(bmp);

                                using (Bitmap bCopy1 = new Bitmap(bmp))
                                using (Bitmap bCopy2 = new Bitmap(bmp))
                                {
                                    MorphologicalProcessing2.IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.Dilate();
                                    alg.BGW = this.backgroundWorker4;
                                    alg.SetupEx(kernelLength, kernelLength);
                                    alg.ApplyGrayscale(bCopy1);
                                    alg.Dispose();

                                    alg = new MorphologicalProcessing2.Algorithms.Erode();
                                    alg.BGW = this.backgroundWorker4;
                                    alg.SetupEx(kernelLength, kernelLength);
                                    alg.ApplyGrayscale(bCopy2);
                                    alg.Dispose();

                                    Bitmap bOut = new Bitmap(bCopy1.Width, bCopy1.Height);
                                    Subtract(bCopy1, bCopy2, bOut);

                                    iG = bOut;
                                }
                            }

                            if (useChainCode)
                            {
                                //now get the components and fill the inner parts of the components white,
                                //so these pixels will not be removed from the result.
                                using Bitmap iGC = new Bitmap(iG ?? throw new ArgumentNullException("iG is null"));
                                Grayscale(iGC);
                                Fipbmp fip = new Fipbmp();
                                fip.ReplaceColors(iGC, 0, 0, 0, 0, tolerance, 255, 0, 0, 0);

                                List<ChainCode>? c = GetBoundary(iGC, 0, false);
                                if (c != null)
                                {
                                    c = c.OrderByDescending(x => x.Coord.Count).ToList();

                                    foreach (ChainCode cc in c)
                                    {
                                        if (!ChainFinder.IsInnerOutline(cc))
                                        {
                                            using Graphics gx = Graphics.FromImage(iG);
                                            using GraphicsPath gP = new GraphicsPath();
                                            gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                            gP.FillMode = FillMode.Winding;
                                            gx.FillPath(Brushes.White, gP);
                                        }
                                        else
                                        {
                                            if (procInner)
                                            {
                                                using Graphics gx = Graphics.FromImage(iG);
                                                using (GraphicsPath gP = new GraphicsPath())
                                                {
                                                    try
                                                    {
                                                        gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());

                                                        if (setInnerTransp)
                                                        {
                                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                                            gx.FillPath(Brushes.Transparent, gP);
                                                        }
                                                        else
                                                        {
                                                            using Bitmap bC = new Bitmap(iG);
                                                            using TextureBrush tb = new TextureBrush(bC);
                                                            gx.FillPath(tb, gP);
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

                            if (postBlur && iG != null)
                            {
                                if (this.backgroundWorker4.WorkerReportsProgress)
                                    this.backgroundWorker4.ReportProgress(0);
                                igg.FastZGaussian_Blur_NxN_SigmaAsDistance(iG, pBKrnl, 0.01,
                                                                        255, true, true, false);
                            }
                            e.Result = iG;
                        }
                    }
                }
            }
        }

        public unsafe void Subtract(Bitmap bCopy1, Bitmap bCopy2, Bitmap bOut)
        {
            int w = bOut.Width;
            int h = bOut.Height;
            BitmapData bmD1 = bCopy1.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmD2 = bCopy2.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmOut = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD1.Stride;

            Parallel.For(0, h, y =>
            {
                byte* pC1 = (byte*)bmD1.Scan0;
                byte* pC2 = (byte*)bmD2.Scan0;
                byte* p = (byte*)bmOut.Scan0;

                pC1 += y * stride;
                pC2 += y * stride;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    byte b = (byte)Math.Max(Math.Min((int)pC1[0] - pC2[0], 255), 0);
                    byte g = (byte)Math.Max(Math.Min((int)pC1[1] - pC2[1], 255), 0);
                    byte r = (byte)Math.Max(Math.Min((int)pC1[2] - pC2[2], 255), 0);

                    p[0] = b;
                    p[1] = g;
                    p[2] = r;
                    p[3] = 255;

                    pC1 += 4;
                    pC2 += 4;
                    p += 4;
                }
            });

            bCopy1.UnlockBits(bmD1);
            bCopy2.UnlockBits(bmD2);
            bOut.UnlockBits(bmOut);
        }

        private unsafe void Grayscale(Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            int nWidth = bmp.Width;
            int nHeight = bmp.Height;

            byte* p = (byte*)bmData.Scan0;

            int pos = 0;
            for (int y = 0; y < nHeight; y++)
            {
                pos = y * stride;
                for (int x = 0; x < nWidth; x++)
                {
                    int v = (int)Math.Max(Math.Min((double)p[pos] * 0.11 + (double)p[pos + 1] * 0.59 + (double)p[pos + 2] * 0.3, 255), 0);
                    p[pos] = p[pos + 1] = p[pos + 2] = (byte)v;

                    pos += 4;
                }
            }

            bmp.UnlockBits(bmData);
        }

        private void backgroundWorker4_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= this.toolStripProgressBar1.Maximum)
                this.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bmp = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                this._undoOPCache?.Add(bmp);

                this._pic_changed = true;

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.SetControls(true);
                this.btnUndo.Enabled = true;

                this.CheckRedoButton();

                this.btnInvGaussGrad.Text = "Go";

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();
                this.toolStripProgressBar1.Value = 100;
                this.Timer3.Start();

                this.backgroundWorker4.Dispose();
                this.backgroundWorker4 = new BackgroundWorker();
                this.backgroundWorker4.WorkerReportsProgress = true;
                this.backgroundWorker4.WorkerSupportsCancellation = true;
                this.backgroundWorker4.DoWork += backgroundWorker4_DoWork;
                this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
                this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            }
        }

        private void cbSecondColor_CheckedChanged(object sender, EventArgs e)
        {
            this.numValSrc2.Enabled = this.numValDst2.Enabled = this.cbSecondColor.Checked;
        }

        private void rbIGG_CheckedChanged(object sender, EventArgs e)
        {
            this.label22.Enabled = this.label21.Enabled = this.numIGGAlpha.Enabled = this.numIGGDivisor.Enabled = this.rbIGG.Checked;
        }

        private void cbDoSecondMult_CheckedChanged(object sender, EventArgs e)
        {
            this.cbAuto.Enabled = this.rbLessThan.Enabled = this.rbGreaterThan.Enabled = this.label9.Enabled =
                this.label2.Enabled = this.label6.Enabled = this.numTh.Enabled = this.numThMultiplier.Enabled =
                this.label3.Enabled = this.label5.Enabled = this.numF2.Enabled = this.numExp2.Enabled = this.cbDoSecondMult.Checked;
        }

        private void cbDoFirstMult_CheckedChanged(object sender, EventArgs e)
        {
            this.label1.Enabled = this.label2.Enabled = this.numF1.Enabled = this.numExp1.Enabled = cbDoFirstMult.Checked;
        }

        private List<ChainCode>? GetBoundary(Bitmap? upperImg, int minAlpha, bool grayScale)
        {
            List<ChainCode>? l = null;
            if (upperImg != null)
            {
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
                    l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, grayScale, 0, false, 0, false);
                }
                catch
                {

                }
                finally
                {
                    if (bmpTmp != null)
                    {
                        bmpTmp.Dispose();
                        bmpTmp = null;
                    }
                }
            }
            return l;
        }

        private void cbUseChainCode_CheckedChanged(object sender, EventArgs e)
        {
            this.label11.Enabled = this.numTolerance.Enabled =
                this.cbSetInnerTransp.Enabled = this.cbProcInner.Enabled = this.cbUseChainCode.Checked;
        }
    }
}
