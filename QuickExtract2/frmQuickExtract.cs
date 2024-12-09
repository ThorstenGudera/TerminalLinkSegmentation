using AvailMem;
using Cache;
using QuickExtractingLib2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace QuickExtract2
{
    //roughly following:
    //Interactive Segmentation with Intelligent Scissors
    //Eric N.Mortensen
    //William A.Barrett#
    //https://courses.cs.washington.edu/courses/cse455/09wi/readings/seg_scissors.pdf
    public partial class frmQuickExtract : Form
    {
        private Bitmap? _bmpBU;
        private int _bmpWidth;
        private int _bmpHeight;
        //private bool _zoomWidth;
        //private bool _dontDrawPath;
        private Color _zColor = Color.Yellow;
        private Color _fColor = Color.Orange;
        private List<List<PointF>>? _selectedPath;
        private List<RectangleF>? tempDataZ;
        private Color _highlightColor = Color.Lime;
        private List<int>? _checkedPaths;
        private bool _dontDoMouseMove;
        private bool _pathClosed;
        private bool firstClick;
        private int _ix;
        private int _iy;

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
                Bitmap b = this.helplineRulerCtrl1.Bmp;
                if (b == null)
                    b = this.helplineRulerCtrl1.Bmp;
                return b;
            }
        }
        private bool _pic_changed = false;

        private int _openPanelHeight;
        private bool _isHoveringVertically;
        private bool _doubleClicked;
        private bool _dontDoZoom;
        private Bitmap? imgDataPic;
        private Bitmap? bmpForValueComputation;
        private Point curPtBU2;
        private PointF _tempLast;
        //private int _amountX;
        //private int _amountY;
        private List<Point>? tempData;

        private List<List<PointF>>? _curPathCopy;
        private bool _cloneCurPath;
        private PointF _spBU;
        private int _frm3MinDist;
        private int _frm3Epsilon;
        private bool _frm3ReducePoints;
        private bool _dontUpdateTempData;

        private object _lockObject = new object();
        private RectangleF[]? _tempDataZA;

        public frmQuickExtract(Bitmap bmp)
        {
            InitializeComponent();

            HelperFunctions.HelperFunctions.SetFormSizeBig(this);
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

            this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
            this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));
            //this._zoomWidth = false;

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.AddDefaultHelplines();
            this.helplineRulerCtrl1.ResetAllHelpLineLabelsColor();

            this.helplineRulerCtrl1.dbPanel1.DragOver += bitmapPanel1_DragOver;
            this.helplineRulerCtrl1.dbPanel1.DragDrop += bitmapPanel1_DragDrop;

            this.helplineRulerCtrl1.dbPanel1.MouseDown += Helplinerulerctrl1_MouseDown;
            this.helplineRulerCtrl1.dbPanel1.MouseMove += Helplinerulerctrl1_MouseMove;
            this.helplineRulerCtrl1.dbPanel1.MouseUp += Helplinerulerctrl1_MouseUp;
            this.helplineRulerCtrl1.PostPaint += Helplinerulerctrl1_Paint;

            this.firstClick = true;
            this.imgDataPic = new Bitmap(bmp);
            this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

            this.quickExtractingCtrl1.btnClosePath.Click += Button1_Click;
            this.quickExtractingCtrl1.btnNewPath.Click += Button3_Click;
            this.quickExtractingCtrl1.btnSavedPaths.Click += Button30_Click;
            this.quickExtractingCtrl1.btnRemSeg.Click += Button6_Click;
            this.quickExtractingCtrl1.btnRemPoint.Click += Button7_Click;
            this.quickExtractingCtrl1.btnRS.Click += Button27_Click;
            this.quickExtractingCtrl1.btnResetPath.Click += Button9_Click;
            this.quickExtractingCtrl1.btnEditPathPoints.Click += Button24_Click;
            this.quickExtractingCtrl1.btnStop.Click += Button12_Click;
            this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
            this.quickExtractingCtrl1.cbR.CheckedChanged += CheckBox1_CheckedChanged;
            this.quickExtractingCtrl1.cbG.CheckedChanged += CheckBox1_CheckedChanged;
            this.quickExtractingCtrl1.cbB.CheckedChanged += CheckBox1_CheckedChanged;
            this.quickExtractingCtrl1.numPenWidth.ValueChanged += NumericUpDown1_ValueChanged;
            this.quickExtractingCtrl1.btnPenCol.Click += Button13_Click;
            this.quickExtractingCtrl1.cbRunAlg.CheckedChanged += CheckBox4_CheckedChanged;
            this.quickExtractingCtrl1.cbRunOnMouseDown.CheckedChanged += CheckBox5_CheckedChanged;
            this.quickExtractingCtrl1.btnCrop.Click += Button25_Click;
            this.quickExtractingCtrl1.btnGo1234.Click += Button31_Click;

            this.quickExtractingCtrl1.numValL.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numValM.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numValG.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numLapTh.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numEdgeWeight.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numValCl.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numValC0l.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numValP.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numVal_I.ValueChanged += NumericUpDown2_ValueChanged;
            this.quickExtractingCtrl1.numValO.ValueChanged += NumericUpDown2_ValueChanged;

            this.quickExtractingCtrl1.btnStdVals.Click += Button34_Click;
            this.quickExtractingCtrl1.btnStdValsLow.Click += Button35_Click;
            this.quickExtractingCtrl1.cbAddLine.CheckedChanged += CheckBox21_CheckedChanged;
            this.quickExtractingCtrl1.cbScaleValues.CheckedChanged += CheckBox9_CheckedChanged;
            this.quickExtractingCtrl1.btnTrain.Click += Button14_Click;
            this.quickExtractingCtrl1.btnResetTrain.Click += Button15_Click;
        }


        private void Helplinerulerctrl1_Paint(object sender, PaintEventArgs e)
        {
            //if (!this._dontDrawPath)
            {
                int count = 0;
                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (this.quickExtractingCtrl1.TempPath != null && this.quickExtractingCtrl1.TempPath.Count > 1)
                        gp.AddLines(this.quickExtractingCtrl1.TempPath.ToArray());
                    float w = System.Convert.ToSingle(this.quickExtractingCtrl1.numPenWidth.Value);
                    int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                    int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                    using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                    {
                        gp.Transform(m);
                        using (Pen pen = new Pen(new SolidBrush(_zColor), w))
                        {
                            e.Graphics.DrawPath(pen, gp);
                            count += gp.PointCount;
                        }
                    }
                }

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
                    {
                        for (int i = 0; i <= this.quickExtractingCtrl1.CurPath.Count - 1; i++)
                        {
                            List<PointF> p = this.quickExtractingCtrl1.CurPath[i];
                            if (p != null && p.Count > 1)
                                gp.AddLines(p.ToArray());
                        }
                    }
                    float w = System.Convert.ToSingle(this.quickExtractingCtrl1.numPenWidth.Value);
                    int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                    int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                    using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                    {
                        gp.Transform(m);
                        using (Pen pen = new Pen(new SolidBrush(this._fColor), w))
                        {
                            e.Graphics.DrawPath(pen, gp);
                            this.quickExtractingCtrl1.Label43.Text = (count + gp.PointCount).ToString() + " - points";
                        }
                    }
                }

                if (tempDataZ != null && tempDataZ.Count > 0 && !this._dontUpdateTempData)
                {
                    if (this._tempDataZA != null && this._tempDataZA.Length > 0)
                    {
                        try
                        {
                            e.Graphics.TranslateTransform(this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                            e.Graphics.FillRectangles(Brushes.Lime, this._tempDataZA);
                            e.Graphics.ResetTransform();
                        }
                        catch //(Exception ex)
                        {
                            //MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }

            if (this._selectedPath != null)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (this._selectedPath.Count > 0)
                    {
                        for (int i = 0; i < this._selectedPath.Count; i++)
                        {
                            List<PointF> p = this._selectedPath[i];
                            if (p != null && p.Count > 1)
                                gp.AddLines(p.ToArray());
                        }
                    }

                    float w = System.Convert.ToSingle(this.quickExtractingCtrl1.numPenWidth.Value);
                    int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                    int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                    using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                    {
                        gp.Transform(m);
                        using (Pen pen = new Pen(new SolidBrush(this._highlightColor), w))
                        {
                            e.Graphics.DrawPath(pen, gp);
                            this.quickExtractingCtrl1.Label43.Text = gp.PointCount.ToString() + " - points";
                        }
                    }
                }
            }

            if (this._checkedPaths != null && this._checkedPaths.Count > 0)
            {
                int cnt = 0;
                for (int j = 0; j < this._checkedPaths.Count; j++)
                {
                    if (this._checkedPaths[j] == -1)
                    {
                        // curpath
                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
                            {
                                for (int i = 0; i <= this.quickExtractingCtrl1.CurPath.Count - 1; i++)
                                {
                                    List<PointF> p = this.quickExtractingCtrl1.CurPath[i];
                                    if (p != null && p.Count > 1)
                                        gp.AddLines(p.ToArray());
                                }
                            }

                            float w = System.Convert.ToSingle(this.quickExtractingCtrl1.numPenWidth.Value);
                            int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                            int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                            using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                            {
                                gp.Transform(m);
                                using (Pen pen = new Pen(new SolidBrush(this._highlightColor), w))
                                {
                                    e.Graphics.DrawPath(pen, gp);
                                    cnt += gp.PointCount;
                                }
                            }
                        }
                    }
                    else if (this.quickExtractingCtrl1.PathList != null)
                    {
                        List<List<PointF>> cPath = this.quickExtractingCtrl1.PathList[this._checkedPaths[j]];

                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            if (cPath.Count > 0)
                            {
                                for (int i = 0; i <= cPath.Count - 1; i++)
                                {
                                    List<PointF> p = cPath[i];
                                    if (p != null && p.Count > 1)
                                        gp.AddLines(p.ToArray());
                                }
                            }

                            float w = System.Convert.ToSingle(this.quickExtractingCtrl1.numPenWidth.Value);
                            int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                            int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                            using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                            {
                                gp.Transform(m);
                                using (Pen pen = new Pen(new SolidBrush(this._highlightColor), w))
                                {
                                    e.Graphics.DrawPath(pen, gp);
                                    cnt += gp.PointCount;
                                }
                            }
                        }
                    }
                }
                this.quickExtractingCtrl1.Label43.Text = cnt.ToString() + " - points";
            }
        }

        private void Helplinerulerctrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.quickExtractingCtrl1.cbRunOnMouseDown.Checked = !this.quickExtractingCtrl1.cbRunOnMouseDown.Checked;
            if (this.quickExtractingCtrl1.cbRunOnMouseDown.Checked == false && this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();
        }

        private void Helplinerulerctrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                // && !this._dontDoMove

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

                    toolStripStatusLabel1.Text = "x: " + _ix.ToString() + ", y: " + _iy.ToString() + " - " + "GrayValue (all channels): " + System.Convert.ToInt32(Math.Min(System.Convert.ToDouble(c.B) * 0.11 + System.Convert.ToDouble(c.G) * 0.59 + System.Convert.ToDouble(c.R) * 0.3, 255)).ToString() + " - RGB: " + c.R.ToString() + ";" + c.G.ToString() + ";" + c.B.ToString();

                    this.ToolStripStatusLabel2.BackColor = c;

                    if (!_dontDoMouseMove && this.firstClick == false && this.quickExtractingCtrl1.cbRunOnMouseDown.Checked && this.quickExtractingCtrl1.cbRunAlg.Checked)
                    {
                        if (this.timer1.Enabled)
                            this.timer1.Stop();
                        this.timer1.Start();
                    }
                }
            }
        }

        private void Helplinerulerctrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.curPtBU2.X == System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom) && this.curPtBU2.Y == System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom))
                {
                    if (this.quickExtractingCtrl1.SeedPoints != null && this.quickExtractingCtrl1.Alg != null && !this.firstClick && _dontDoMouseMove == false && this._pathClosed == false)
                    {
                        if (this.backgroundWorker1.IsBusy)
                            this.backgroundWorker1.CancelAsync();

                        cleanCP();
                        this.quickExtractingCtrl1.AddTempPath(true, true, _ix, _iy);
                        this.quickExtractingCtrl1.Alg.ReInit(System.Convert.ToInt32(this.quickExtractingCtrl1.SeedPoints[this.quickExtractingCtrl1.SeedPoints.Count - 1].X), System.Convert.ToInt32(this.quickExtractingCtrl1.SeedPoints[this.quickExtractingCtrl1.SeedPoints.Count - 1].Y), this.quickExtractingCtrl1.cbScaleValues.Checked, this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex);

                        this.quickExtractingCtrl1.TempPath = new List<PointF>();

                        this._ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                        this._iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        return;
                    }
                }

                this._ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                this._iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                if (e.Button == MouseButtons.Left && this.quickExtractingCtrl1.cbRunAlg.Checked && _dontDoMouseMove == false)
                {
                    _dontDoMouseMove = true;
                    apply();
                }
            }
        }

        private void CheckBox12_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (this.CheckBox12.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Check the CheckBox \"run alg on click\".\nSet the parameters and set points by clicking onto the Image (close to the edges which are the outline(s) of the shape(s) to extract).\n\nTo make the algorithm clamp to edges more, increase valM much (close to, or to 1.0), and keep valG low.\nAlso you could set valL to a low value and the Laplacian Threshold to a high value. Or set the valCl value to a high value around 1.0.");
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

                    if (this._bmpBU != null)
                        try
                        {
                            if (AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 12L))
                                b1 = (Bitmap)this._bmpBU.Clone();
                            else
                                throw new Exception();

                            if (this.quickExtractingCtrl1.Alg != null)
                                this.quickExtractingCtrl1.Alg.DisposeBmpData();
                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");

                            this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

                            this._pic_changed = false;

                            this.helplineRulerCtrl1.CalculateZoom();

                            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                            // SetHRControlVars();

                            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                            this.helplineRulerCtrl1.dbPanel1.Invalidate();

                            this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
                            this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;

                            _undoOPCache?.Reset(false);
                            this.button4.Enabled = false;

                            if (_undoOPCache?.Count > 1)
                                this.button5.Enabled = true;
                            else
                                this.button5.Enabled = false;
                        }
                        catch
                        {
                            if (b1 != null)
                                b1.Dispose();
                        }
                }
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition);
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
        }

        private void bitmapPanel1_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void bitmapPanel1_DragDrop(object? sender, DragEventArgs e)
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

                                if (this.quickExtractingCtrl1.Alg != null)
                                    this.quickExtractingCtrl1.Alg.DisposeBmpData();
                                this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                                this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

                                _undoOPCache?.Clear(this.helplineRulerCtrl1.Bmp);

                                _pic_changed = false;

                                double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                                double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                                if (multiplier >= faktor)
                                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                                else
                                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));
                                //this._zoomWidth = false;

                                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                                this.button4.Enabled = this.button5.Enabled = false;

                                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                                this.Text = files[0] + " - frmQuickExtract";

                                this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
                                this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;

                                this.firstClick = true;
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
            _undoOPCache?.CheckRedoButton(this.button5);
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
                    if (pi != null)
                        pi.SetValue(ct, bitmapToBeSet);
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

        private void apply()
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                int ix = this._ix;
                int iy = this._iy;

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex == 0 ? "run" : "run2",
                    bmpDataForValueComputation = this.bmpForValueComputation,
                    imgDataPic = this.imgDataPic,
                    MouseClicked = true,
                    Ramps = this.quickExtractingCtrl1.Ramps,
                    SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                    TempPath = this.quickExtractingCtrl1.TempPath,
                    CurPath = this.quickExtractingCtrl1.CurPath,
                    valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                    valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                    valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                    valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                    valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                    valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                    doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                    doR = this.quickExtractingCtrl1.cbR.Checked,
                    doG = this.quickExtractingCtrl1.cbG.Checked,
                    doB = this.quickExtractingCtrl1.cbB.Checked,
                    neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                    useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value)
                };

                if (this.quickExtractingCtrl1.cbAddLine.Checked)
                    this.PrepareBGW(new object[] { 101, new Point(ix, iy), @params });
                else if (this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex == 0)
                    this.PrepareBGW(new object[] { 1, new Point(ix, iy), @params });
                else
                    this.PrepareBGW(new object[] { 6, new Point(ix, iy), @params });
            }
        }

        private Color GetCorrespondingColor(Color fColor)
        {
            int r = fColor.R > 127 ? fColor.R - 127 : fColor.R + 127;
            int g = fColor.G > 127 ? fColor.G - 127 : fColor.G + 127;
            int b = fColor.B > 127 ? fColor.B - 127 : fColor.B + 127;
            Color c = Color.FromArgb(255, Math.Max(Math.Min(r, 255), 0), Math.Max(Math.Min(g, 255), 0), Math.Max(Math.Min(b, 255), 0));
            return c;
        }

        // helper, will be removed, when everything runs correctly
        private void cleanCP()
        {
            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
            {
                for (int i = this.quickExtractingCtrl1.CurPath.Count - 1; i >= 0; i += -1)
                {
                    if (this.quickExtractingCtrl1.CurPath[i].Count == 0)
                        this.quickExtractingCtrl1.CurPath.RemoveAt(i);
                }

                // 'check for seedpoint outliers '07.06.2022
                // If Me.ComboBox1.SelectedIndex = 1 Then
                // For i As Integer = Me.CurPath.Count - 1 To 1 Step -1
                // Dim p As List(Of PointF) = Me.CurPath(i)
                // Dim q As List(Of PointF) = Me.CurPath(i - 1)

                // Dim pt1 As PointF = p(0)
                // Dim pt2 As PointF = q(q.Count - 1)

                // Dim dx As Double = pt1.X - pt2.X
                // Dim dy As Double = pt1.Y - pt2.Y

                // Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)

                // If dist > 2.0 Then
                // Me.CurPath.RemoveAt(i)
                // End If
                // Next
                // End If

                // 'check for seedpoint outliers '07.06.2022
                // If Me.SeedPoints IsNot Nothing AndAlso Me.SeedPoints.Count > 1 Then
                // For i As Integer = Me.SeedPoints.Count - 1 To 1 Step -1
                // If Me.SeedPoints(i).X = Me.SeedPoints(i - 1).X AndAlso
                // Me.SeedPoints(i).Y = Me.SeedPoints(i - 1).Y Then

                // Me.SeedPoints.RemoveAt(i)
                // End If

                // Dim found As Boolean = False

                // For j As Integer = 0 To Me.CurPath.Count - 1
                // Dim p As List(Of PointF) = Me.CurPath(j)
                // Dim pt As PointF = p(p.Count - 1)

                // Dim r As New RectangleF(pt.X - 1, pt.Y - 1, 3, 3)

                // If Me.SeedPoints.Count > i AndAlso r.Contains(Me.SeedPoints(i)) Then
                // found = True
                // Exit For
                // End If

                // 'Dim dx As Double = p(p.Count - 1).X - pt.X
                // 'Dim dy As Double = p(p.Count - 1).Y - pt.Y
                // 'Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)

                // 'If Me.SeedPoints.Count > i AndAlso dist <= Math.Sqrt(2.0) Then
                // '    found = True
                // '    Exit For
                // 'End If
                // Next

                // If Me.SeedPoints.Count > i AndAlso i > 1 AndAlso found = False Then
                // Me.SeedPoints.RemoveAt(i)
                // End If
                // Next
                // End If

                // For i As Integer = 0 To Me.CurPath.Count - 2
                // Dim p As List(Of PointF) = Me.CurPath(i)
                // Dim q As List(Of PointF) = Me.CurPath(i + 1)

                // If p.Count > 0 Then
                // Dim dx As Double = p(p.Count - 1).X - q(0).X
                // Dim dy As Double = p(p.Count - 1).Y - q(0).Y
                // Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)
                // If dist > Math.Sqrt(2.0) Then
                // p.Add(q(0))
                // End If
                // End If
                // Next

                // refactor into method
                if (this.quickExtractingCtrl1.CurPath.Count > 0)
                {
                    this.quickExtractingCtrl1.SeedPoints = new List<PointF>();

                    if (this.quickExtractingCtrl1.CurPath[0].Count > 0)
                    {
                        PointF pt = this.quickExtractingCtrl1.CurPath[0][0];
                        this.quickExtractingCtrl1.SeedPoints.Add(new PointF(pt.X, pt.Y));
                    }

                    for (int j = 0; j < this.quickExtractingCtrl1.CurPath.Count; j++)
                    {
                        if (this.quickExtractingCtrl1.CurPath[j].Count > 0)
                        {
                            PointF pt = this.quickExtractingCtrl1.CurPath[j][this.quickExtractingCtrl1.CurPath[j].Count - 1];
                            this.quickExtractingCtrl1.SeedPoints.Add(new PointF(pt.X, pt.Y));
                        }
                        else
                            MessageBox.Show("ffff");
                    }
                }
            }
        }

        private bool checkPath()
        {
            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0 && this.quickExtractingCtrl1.CurPath[0].Count > 0 && this.helplineRulerCtrl1.Bmp != null)
            {
                PointF sp = this.quickExtractingCtrl1.CurPath[0][0];

                int stride = this.helplineRulerCtrl1.Bmp.Width * 4;

                int countF = 0;
                if (this.quickExtractingCtrl1.CurPath.Count > 1)
                {
                    for (int i = 0; i <= this.quickExtractingCtrl1.CurPath.Count - 3; i++)
                        countF += this.quickExtractingCtrl1.CurPath[i].Count;
                }

                for (int l = this.quickExtractingCtrl1.CurPath.Count - 1; l >= 0; l += -1)
                {
                    List<PointF> ListF = this.quickExtractingCtrl1.CurPath[l];
                    int j = -1;
                    for (int i = 0; i <= ListF.Count - 1; i++)
                    {
                        double dx = sp.X - ListF[i].X;
                        double dy = sp.Y - ListF[i].Y;

                        // alternativ
                        // if((dx == 0 && Math.abs(dy) == 1) || (Math.abs(dx) == 1) && dy == 0)

                        if (Math.Sqrt(dx * dx + dy * dy) <= 1)
                        {
                            j = i + 1;

                            if (this.quickExtractingCtrl1.CurPath.Count == 1 || l == 0)
                            {
                                countF = j;
                                break;
                            }
                        }
                    }

                    if ((j > -1 && countF > 3))
                    {
                        ListF.RemoveRange(j, ListF.Count - j);
                        ListF.Add(sp);

                        if (l < this.quickExtractingCtrl1.CurPath.Count - 1)
                            this.quickExtractingCtrl1.CurPath.RemoveRange(l + 1, this.quickExtractingCtrl1.CurPath.Count - (l + 1));

                        return true;
                    }
                }
            }

            return false;
        }

        private void PrepareBGW(object[] args)
        {
            int num = System.Convert.ToInt32(args[0]);
            Point pt = (Point)args[1];
            QuickExtractingParameters @params = (QuickExtractingParameters)args[2];

            this.Cursor = Cursors.WaitCursor;

            cleanCP(); // checkPath here for consistency of seedpoints

            switch (num)
            {
                case 1:
                    {
                        if ((this.quickExtractingCtrl1.Alg == null || this.firstClick))
                        {
                            this.quickExtractingCtrl1.Alg = new QuickExtractingAlg(@params.imgDataPic, @params.bmpDataForValueComputation, pt, @params.doR, @params.doG, @params.doB, @params.doScale, @params.neighbors, this.backgroundWorker1);
                            this.quickExtractingCtrl1.Alg.SetColorBmp(true);
                            this.quickExtractingCtrl1.Alg.NotifyEdges += Alg_NotifyEdges;
                        }

                        if (this.quickExtractingCtrl1.Alg.picAlg?.CostMaps == null && @params.useCostMap)
                            this.quickExtractingCtrl1.Alg.InitCostMapStandardCalc();
                        else if ((!@params.useCostMap))
                            if (this.quickExtractingCtrl1.Alg.picAlg != null)
                                this.quickExtractingCtrl1.Alg.picAlg.CostMaps = null;

                        if (this.quickExtractingCtrl1.Alg.picAlg != null && this.quickExtractingCtrl1.Alg.picAlg.CostMaps != null && @params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0)
                            this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                        this.quickExtractingCtrl1.Alg.valL = @params.valL;
                        this.quickExtractingCtrl1.Alg.valM = @params.valM;
                        this.quickExtractingCtrl1.Alg.valG = @params.valG;
                        this.quickExtractingCtrl1.Alg.laplTh = @params.laplTh;
                        this.quickExtractingCtrl1.Alg.edgeWeight = @params.edgeWeight;
                        this.quickExtractingCtrl1.Alg.dist = @params.dist;

                        this.quickExtractingCtrl1.Alg.CurPath = @params.CurPath;
                        this.quickExtractingCtrl1.Alg.SeedPoints = @params.SeedPoints;

                        this.quickExtractingCtrl1.Alg.valP = @params.valP;
                        this.quickExtractingCtrl1.Alg.valI = @params.valI;
                        this.quickExtractingCtrl1.Alg.valO = @params.valO;

                        this.quickExtractingCtrl1.Alg.valCl = @params.valCl;
                        this.quickExtractingCtrl1.Alg.valCol = @params.valCol;

                        this.quickExtractingCtrl1.Alg.NotifyEach = @params.notifyEach;

                        if (!this.firstClick)
                        {
                            if (!this.backgroundWorker1.IsBusy && quickExtractingCtrl1.Alg.SeedPoints != null && quickExtractingCtrl1.Alg.SeedPoints.Count > 0)
                            {
                                this.quickExtractingCtrl1.Alg.MouseClicked = @params.MouseClicked;
                                this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint = new Point(System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].Y));
                                this.quickExtractingCtrl1.Alg.ReInit(pt.X, pt.Y, @params.doScale, @params.neighbors);

                                bool found = false;

                                if (!this.quickExtractingCtrl1.Alg.MouseClicked)
                                {
                                    for (int j = 0; j <= quickExtractingCtrl1.Alg.SeedPoints.Count - 1; j++)
                                    {
                                        if (quickExtractingCtrl1.Alg.SeedPoints[j].X == pt.X && quickExtractingCtrl1.Alg.SeedPoints[j].Y == pt.Y)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }

                                if (!found && quickExtractingCtrl1.Alg.MouseClicked)
                                    quickExtractingCtrl1.Alg.SeedPoints.Add(new Point(pt.X, pt.Y));

                                this.quickExtractingCtrl1.btnNewPath.Enabled = false;
                                this.backgroundWorker1.RunWorkerAsync(new object[] { 1, pt, @params });
                            }
                        }
                        else
                        {
                            this.quickExtractingCtrl1.Alg?.SeedPoints?.Add(new Point(pt.X, pt.Y));
                            this.firstClick = false;
                            this.Cursor = Cursors.Default;
                            _dontDoMouseMove = false;
                        }

                        break;
                    }

                case 2:
                    {
                        if (this.quickExtractingCtrl1.Alg != null & this.backgroundWorker1.IsBusy == false)
                        {
                            this.quickExtractingCtrl1.Alg?.PrepareForTranslatedPaths(@params.imgDataPic, @params.bmpDataForValueComputation, @params.amountX, @params.amountY);
                            this.quickExtractingCtrl1.Alg?.SetColorBmp(true);
                        }
                        this.Cursor = Cursors.Default;
                        _dontDoMouseMove = false;
                        break;
                    }

                case 3:
                    {
                        if (this.quickExtractingCtrl1.Alg != null && quickExtractingCtrl1.Alg.SeedPoints != null && this.quickExtractingCtrl1.Alg.picAlg?.CostMaps != null && this.imgDataPic != null)
                        {
                            this.quickExtractingCtrl1.Alg.MouseClicked = false;
                            this.quickExtractingCtrl1.Alg.bgw = this.backgroundWorker1;
                            this.quickExtractingCtrl1.Alg.SeedPoints = @params.SeedPoints;

                            if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0)
                                this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                            this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint = new Point(System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints?[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints?[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].Y));

                            if (this.quickExtractingCtrl1.Alg.SortedPixelList?.Count == 0)
                            {
                                Stack<int> addresses = new Stack<int>();
                                addresses.Push(pt.Y * this.imgDataPic.Width * 4 + pt.X * 4);
                                quickExtractingCtrl1.Alg.SortedPixelList.Add(new QEData(0, addresses));
                            }

                            if (!this.backgroundWorker1.IsBusy)
                            {
                                this.quickExtractingCtrl1.Alg.NotifyEach = @params.notifyEach;
                                this.quickExtractingCtrl1.btnNewPath.Enabled = false;
                                this.backgroundWorker1.RunWorkerAsync(new object[] { 3, pt, @params });
                            }
                            else
                            {
                                // if the worker is busy and the mouse moved, repeat the last request
                                if (this.timer1.Enabled)
                                    this.timer1.Stop();
                                this.timer1.Start();
                            }
                        }

                        break;
                    }

                case 4:
                    {
                        if (this.quickExtractingCtrl1.Alg?.CurPath != null)
                        {
                            if (this.quickExtractingCtrl1.Alg != null && this.quickExtractingCtrl1.Alg.CurPath.Count > 0 && this.quickExtractingCtrl1.Alg.bmp != null && this.quickExtractingCtrl1.Alg.picAlg != null)
                            {
                                List<PointF> l = this.quickExtractingCtrl1.Alg.CurPath[this.quickExtractingCtrl1.Alg.CurPath.Count - 1]; // todo: take the list with maximum edgeWeight

                                this.quickExtractingCtrl1.Alg.picAlg.Mcc = new MagnitudeAndPixelCostCalculatorHistoData();
                                this.quickExtractingCtrl1.Alg.picAlg.Mcc.AddressesGray = l.ConvertAll(a => System.Convert.ToInt32(a.X) + System.Convert.ToInt32(a.Y) * this.helplineRulerCtrl1.Bmp.Width);
                                this.quickExtractingCtrl1.Alg.picAlg.Mcc.BmpDataForValueComputation = quickExtractingCtrl1.Alg.bmpDataForValueComputation;
                                this.quickExtractingCtrl1.Alg.picAlg.Mcc.DataGray = quickExtractingCtrl1.Alg.picAlg.GrayRepresentation;
                                this.quickExtractingCtrl1.Alg.picAlg.Mcc.Dist = @params.dist;
                                this.quickExtractingCtrl1.Alg.picAlg.Mcc.Stride = this.quickExtractingCtrl1.Alg.bmp.Width * 4;

                                MagnitudeAndPixelCostMap? mcm = quickExtractingCtrl1.Alg?.picAlg.Mcc.CalculateRamps();

                                if (this.quickExtractingCtrl1.Alg != null)
                                    this.quickExtractingCtrl1.Alg.picAlg.CostMaps = mcm;

                                @params.Ramps = this.quickExtractingCtrl1.Alg?.picAlg.CostMaps?.Ramps;
                                this.quickExtractingCtrl1.Ramps = @params.Ramps;

                                this.Cursor = Cursors.Default;
                                _dontDoMouseMove = false;

                                this.quickExtractingCtrl1.lblTrainInfo.Text = "trained";
                            }
                        }
                        break;
                    }

                case 5:
                    {
                        if (this.quickExtractingCtrl1.Alg != null && this.quickExtractingCtrl1.Alg.picAlg != null)
                        {
                            this.quickExtractingCtrl1.Alg.picAlg.CostMaps = null;

                            @params.Ramps = null;
                            this.quickExtractingCtrl1.Ramps = null;
                        }

                        this.Cursor = Cursors.Default;
                        _dontDoMouseMove = false;

                        this.quickExtractingCtrl1.lblTrainInfo.Text = "not trained";
                        break;
                    }

                case 6:
                    {
                        if ((this.quickExtractingCtrl1.Alg == null || this.firstClick))
                        {
                            this.quickExtractingCtrl1.Alg = new QuickExtractingAlg(@params.imgDataPic, @params.bmpDataForValueComputation, pt, @params.doR, @params.doG, @params.doB, @params.doScale, @params.neighbors, this.backgroundWorker1);
                            this.quickExtractingCtrl1.Alg.SetColorBmp(true);
                            this.quickExtractingCtrl1.Alg.NotifyEdges += Alg_NotifyEdges;
                        }

                        if (!quickExtractingCtrl1.Alg.AllScanned || (quickExtractingCtrl1.Alg.AllScanned && @params.MouseClicked))
                        {
                            quickExtractingCtrl1.Alg.AllScanned = false;

                            if (this.quickExtractingCtrl1.Alg.picAlg != null && this.quickExtractingCtrl1.Alg.picAlg.CostMaps == null && @params.useCostMap)
                                this.quickExtractingCtrl1.Alg.InitCostMapStandardCalc();
                            else if (this.quickExtractingCtrl1.Alg.picAlg != null && (!@params.useCostMap))
                                this.quickExtractingCtrl1.Alg.picAlg.CostMaps = null;

                            if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0 && this.quickExtractingCtrl1.Alg.picAlg?.CostMaps != null)
                                this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                            this.quickExtractingCtrl1.Alg.valL = @params.valL;
                            this.quickExtractingCtrl1.Alg.valM = @params.valM;
                            this.quickExtractingCtrl1.Alg.valG = @params.valG;
                            this.quickExtractingCtrl1.Alg.laplTh = @params.laplTh;
                            this.quickExtractingCtrl1.Alg.edgeWeight = @params.edgeWeight;
                            this.quickExtractingCtrl1.Alg.dist = @params.dist;

                            this.quickExtractingCtrl1.Alg.CurPath = @params.CurPath;
                            this.quickExtractingCtrl1.Alg.SeedPoints = @params.SeedPoints;

                            this.quickExtractingCtrl1.Alg.valP = @params.valP;
                            this.quickExtractingCtrl1.Alg.valI = @params.valI;
                            this.quickExtractingCtrl1.Alg.valO = @params.valO;

                            this.quickExtractingCtrl1.Alg.valCl = @params.valCl;
                            this.quickExtractingCtrl1.Alg.valCol = @params.valCol;

                            this.quickExtractingCtrl1.Alg.NotifyEach = @params.notifyEach;

                            if (!this.firstClick)
                            {
                                if (!this.backgroundWorker1.IsBusy)
                                {
                                    this.quickExtractingCtrl1.Alg.MouseClicked = @params.MouseClicked;
                                    this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint = new Point(pt.X, pt.Y);
                                    this.quickExtractingCtrl1.Alg.ReInit(System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints?[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints?[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].Y), @params.doScale, @params.neighbors);

                                    quickExtractingCtrl1.Alg.CancelFlag = false;
                                    this.quickExtractingCtrl1.btnNewPath.Enabled = false;
                                    this.backgroundWorker1.RunWorkerAsync(new object[] { 6, pt, @params });
                                }
                            }
                            else
                            {
                                quickExtractingCtrl1.Alg.SeedPoints?.Add(new Point(pt.X, pt.Y));
                                this.firstClick = false;
                                this.Cursor = Cursors.Default;
                                _dontDoMouseMove = false;
                            }
                        }

                        break;
                    }

                case 7:
                    {
                        if (this.quickExtractingCtrl1.Alg != null && this.quickExtractingCtrl1.Alg.bmp != null)
                        {
                            this.quickExtractingCtrl1.Alg.MouseClicked = false;
                            quickExtractingCtrl1.Alg.bgw = this.backgroundWorker1; // 1234567

                            if (quickExtractingCtrl1.Alg.AllScanned && this.quickExtractingCtrl1.Alg.picAlg?.CostMaps != null && this.quickExtractingCtrl1.Alg.BackPointers != null)
                            {
                                this.quickExtractingCtrl1.Alg.CancelFlag = true;

                                if (this.quickExtractingCtrl1.Alg.picAlg.CostMaps == null && @params.useCostMap)
                                    this.quickExtractingCtrl1.Alg.InitCostMapStandardCalc();
                                else if ((!@params.useCostMap))
                                    this.quickExtractingCtrl1.Alg.picAlg.CostMaps = null;

                                if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0 && this.quickExtractingCtrl1.Alg.picAlg.CostMaps != null)
                                    this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                                this.quickExtractingCtrl1.Alg.SeedPoints = @params.SeedPoints;
                                this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint = new Point(pt.X, pt.Y);

                                int aaa = this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint.X + this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint.Y * this.quickExtractingCtrl1.Alg.bmp.Width;
                                int zzz = this.quickExtractingCtrl1.Alg.BackPointers[aaa];

                                if (zzz != -1)
                                {
                                    List<PointF>? Path = this.quickExtractingCtrl1.Alg.GetPath(this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint.X, this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint.Y);
                                    Path?.Reverse();

                                    if (Path != null)
                                    {
                                        this.quickExtractingCtrl1.Alg.TempPath = Path;

                                        if (!this.backgroundWorker1.IsBusy)
                                        {
                                            this.quickExtractingCtrl1.Alg.NotifyEach = @params.notifyEach;
                                            this.quickExtractingCtrl1.btnNewPath.Enabled = false;
                                            this.backgroundWorker1.RunWorkerAsync(new object[] { 7, pt, @params });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.quickExtractingCtrl1.Alg.CancelFlag = true;

                                if (this.quickExtractingCtrl1.Alg.picAlg?.CostMaps == null && @params.useCostMap)
                                    this.quickExtractingCtrl1.Alg.InitCostMapStandardCalc();
                                else if (this.quickExtractingCtrl1.Alg.picAlg != null && (!@params.useCostMap))
                                    this.quickExtractingCtrl1.Alg.picAlg.CostMaps = null;

                                if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0 && this.quickExtractingCtrl1.Alg.picAlg?.CostMaps != null)
                                    this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                                this.quickExtractingCtrl1.Alg.CurPath = @params.CurPath;
                                this.quickExtractingCtrl1.Alg.SeedPoints = @params.SeedPoints;
                                this.quickExtractingCtrl1.Alg.PreviouslyInsertedPoint = new Point(pt.X, pt.Y);

                                if (this.quickExtractingCtrl1.Alg.SortedPixelList?.Count == 0 && this.imgDataPic != null)
                                {
                                    Stack<int> addresses = new Stack<int>();
                                    addresses.Push(pt.Y * this.imgDataPic.Width * 4 + pt.X * 4);
                                    quickExtractingCtrl1.Alg.SortedPixelList.Add(new QEData(0, addresses));
                                }

                                if (!this.backgroundWorker1.IsBusy)
                                {
                                    this.quickExtractingCtrl1.Alg.ReInit(System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints?[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(quickExtractingCtrl1.Alg.SeedPoints?[quickExtractingCtrl1.Alg.SeedPoints.Count - 1].Y), @params.doScale, @params.neighbors);
                                    this.quickExtractingCtrl1.Alg.NotifyEach = @params.notifyEach;
                                    quickExtractingCtrl1.Alg.CancelFlag = false;
                                    this.quickExtractingCtrl1.btnNewPath.Enabled = false;
                                    this.backgroundWorker1.RunWorkerAsync(new object[] { 71, pt, @params });
                                }
                            }
                        }

                        break;
                    }

                case 101:
                    {
                        if (@params.SeedPoints != null && this.firstClick)
                            @params.SeedPoints.Add(new PointF(pt.X, pt.Y));

                        if (@params.SeedPoints != null && @params.SeedPoints.Count > 0 && @params.CurPath != null && !this.firstClick)
                        {
                            @params.SeedPoints.Add(new PointF(pt.X, pt.Y));
                            this.quickExtractingCtrl1.AddLine(@params.CurPath, @params.SeedPoints);

                            this.quickExtractingCtrl1.SeedPoints = @params.SeedPoints;
                            this.quickExtractingCtrl1.CurPath = @params.CurPath;
                        }

                        this.firstClick = false;
                        this.Cursor = Cursors.Default;
                        _dontDoMouseMove = false;

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        break;
                    }

                case 102:
                    {
                        if (@params.SeedPoints != null && @params.SeedPoints.Count > 0)
                        {
                            @params.TempPath = new List<PointF>();
                            this.quickExtractingCtrl1.TempPath = this.quickExtractingCtrl1.AddLine(@params.TempPath, pt, @params.SeedPoints[@params.SeedPoints.Count - 1]);

                            if (this.quickExtractingCtrl1.cbAutoSeeds.Enabled && this.quickExtractingCtrl1.cbAutoSeeds.Checked && this.quickExtractingCtrl1.TempPath?.Count > 0 && this._tempLast.X != this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1].X &&
                                this._tempLast.Y != this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1].Y)
                            {
                                this._tempLast = this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1];
                                if (this.Timer2.Enabled)
                                    this.Timer2.Stop();
                                this.Timer2.Start();
                            }
                        }

                        this.firstClick = false;
                        this.Cursor = Cursors.Default;
                        _dontDoMouseMove = false;

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        break;
                    }

                default:
                    {
                        this.Cursor = Cursors.Default;
                        break;
                    }
            }
        }

        private void Alg_NotifyEdges(object? sender, NotifyEventArgs e)
        {
            QEAlg_Notify(this, e);
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

            if (!e.Cancel)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();
                if (this._bmpBU != null)
                    this._bmpBU.Dispose();

                if (this.quickExtractingCtrl1.Alg != null)
                {
                    this.quickExtractingCtrl1.Alg.NotifyEdges -= Alg_NotifyEdges;
                    this.quickExtractingCtrl1.Alg.Dispose();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex = 0;
            this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex = 0;

            this.helplineRulerCtrl1.dbPanel1.Select();

            this.quickExtractingCtrl1.cbRunAlg.Select();

            this.ComboBox2.Items.Add((0.75F).ToString());
            this.ComboBox2.Items.Add((0.5F).ToString());
            this.ComboBox2.Items.Add((0.25F).ToString());

            this.ComboBox2.SelectedIndex = 4;

            this.CheckBox12_CheckedChanged(this.CheckBox12, new EventArgs());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool translate = false;
            int amountX = 0;
            int amountY = 0;
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                this.button4.Enabled = false;
                this.button4.Refresh();

                Bitmap bOut = _undoOPCache.DoUndo();

                if (bOut != null)
                {
                    //if (_amountX != 0 || _amountY != 0)
                    //{
                    //    if (MessageBox.Show("Re-translate path(s)?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    //        this.TranslatePathAndSeedPoints(_amountX, _amountY);
                    //}

                    if (this.helplineRulerCtrl1.Bmp.Width != bOut.Width || this.helplineRulerCtrl1.Bmp.Height != bOut.Height)
                    {
                        translate = true;
                        amountX = (bOut.Width - this.helplineRulerCtrl1.Bmp.Width) / 2;
                        amountY = (bOut.Height - this.helplineRulerCtrl1.Bmp.Height) / 2;
                    }

                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");

                    if (_undoOPCache.Count == 0)
                        this._pic_changed = false;
                    else
                        this._pic_changed = true;

                    // Me.helplineRulerCtrl1.CalculateZoom()

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    if (_undoOPCache.CurrentPosition > 1)
                        this.button4.Enabled = true;
                    else
                        this.button4.Enabled = false;

                    // this.button3.Enabled = this.button4.Enabled;

                    if (_undoOPCache.Count > 1)
                        this.button5.Enabled = true;
                    else
                        this.button5.Enabled = false;

                    this.CheckBox18.Checked = true;

                    if (translate && this.quickExtractingCtrl1.Alg != null)
                    {
                        if (this.quickExtractingCtrl1.Alg != null)
                            this.quickExtractingCtrl1.Alg.DisposeBmpData();
                        this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                        this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

                        this.TranslatePathAndSeedPoints(amountX, amountY);

                        QuickExtractingParameters @params = new QuickExtractingParameters()
                        {
                            msg = "translate",
                            bmpDataForValueComputation = this.bmpForValueComputation,
                            imgDataPic = this.imgDataPic,
                            MouseClicked = true,
                            Ramps = this.quickExtractingCtrl1.Ramps,
                            SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                            TempPath = this.quickExtractingCtrl1.TempPath,
                            CurPath = this.quickExtractingCtrl1.CurPath,
                            valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                            valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                            valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                            valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                            valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                            valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                            valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                            valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                            laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                            edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                            doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                            doR = this.quickExtractingCtrl1.cbR.Checked,
                            doG = this.quickExtractingCtrl1.cbG.Checked,
                            doB = this.quickExtractingCtrl1.cbB.Checked,
                            neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                            dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                            useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                            notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value),
                            amountX = amountX,
                            amountY = amountY
                        };

                        this.PrepareBGW(new object[] { 2, new Point(_ix, _iy), @params });
                    }
                }
                else
                    MessageBox.Show("Error while undoing.");
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
                this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();
            bool translate = false;
            int amountX = 0;
            int amountY = 0;
            if (_undoOPCache != null)
            {
                this.button5.Enabled = false;
                this.button5.Refresh();

                Bitmap bOut = _undoOPCache.DoRedo();

                if (bOut != null)
                {
                    //if (_amountX != 0 || _amountY != 0)
                    //{
                    //    if (MessageBox.Show("Re-translate path(s)?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    //        this.TranslatePathAndSeedPoints(-_amountX, -_amountY);
                    //}

                    if (this.helplineRulerCtrl1.Bmp.Width != bOut.Width || this.helplineRulerCtrl1.Bmp.Height != bOut.Height)
                    {
                        translate = true;
                        amountX = (bOut.Width - this.helplineRulerCtrl1.Bmp.Width) / 2;
                        amountY = (bOut.Height - this.helplineRulerCtrl1.Bmp.Height) / 2;
                    }

                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");

                    this._pic_changed = true;
                    // Me.helplineRulerCtrl1.CalculateZoom()

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    // Me.helplineRulerCtrl1.dbPanel1.Invalidate()

                    if (_undoOPCache.CurrentPosition > 1)
                        this.button4.Enabled = true;
                    else
                        this.button4.Enabled = false;

                    if (_undoOPCache.CurrentPosition < _undoOPCache.Count)
                        this.button5.Enabled = true;
                    else
                    {
                        this.button5.Enabled = false;
                        this.CheckBox18.Checked = false;
                    }

                    if (translate && this.quickExtractingCtrl1.Alg != null)
                    {
                        if (quickExtractingCtrl1.Alg != null)
                            quickExtractingCtrl1.Alg.DisposeBmpData();
                        this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                        this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

                        this.TranslatePathAndSeedPoints(amountX, amountY);

                        QuickExtractingParameters @params = new QuickExtractingParameters()
                        {
                            msg = "translate",
                            bmpDataForValueComputation = this.bmpForValueComputation,
                            imgDataPic = this.imgDataPic,
                            MouseClicked = true,
                            Ramps = this.quickExtractingCtrl1.Ramps,
                            SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                            TempPath = this.quickExtractingCtrl1.TempPath,
                            CurPath = this.quickExtractingCtrl1.CurPath,
                            valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                            valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                            valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                            valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                            valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                            valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                            valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                            valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                            laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                            edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                            doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                            doR = this.quickExtractingCtrl1.cbR.Checked,
                            doG = this.quickExtractingCtrl1.cbG.Checked,
                            doB = this.quickExtractingCtrl1.cbB.Checked,
                            neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                            dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                            useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                            notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value),
                            amountX = amountX,
                            amountY = amountY
                        };

                        this.PrepareBGW(new object[] { 2, new Point(_ix, _iy), @params });
                        this.CheckBox18.Checked = true;
                    }
                }
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
                this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
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
                this.button4.PerformClick();
                return true;
            }

            if (keyData == (Keys.Y | Keys.Control))
            {
                this.button5.PerformClick();
                return true;
            }

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
                    this._openPanelHeight = this.Panel1.Height;
                    this._doubleClicked = true;
                    this.Panel1.Dock = DockStyle.None;
                    this.panel3.Dock = DockStyle.Top;

                    this._isHoveringVertically = true;

                    this.panel3.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();
                    this.statusStrip1.SendToBack();

                    this.Panel1.BringToFront();

                    this.Panel1.AutoScroll = false;
                }
                else
                {
                    this._doubleClicked = false;
                    this.panel3.Dock = DockStyle.None;
                    this.Panel1.Dock = DockStyle.Top;

                    this.panel3.Width = this.helplineRulerCtrl1.Width = this.Panel1.Width = this.ClientSize.Width;

                    this._isHoveringVertically = false;
                    this.Panel1.Height = this._openPanelHeight;

                    this.statusStrip1.SendToBack();
                    this.Panel1.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();

                    this.Panel1.AutoScroll = true;
                    this.Panel1.BringToFront();
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
                    this.Panel1.Dock = DockStyle.Top;
                    this.panel3.Dock = DockStyle.None;
                    this._isHoveringVertically = false;
                    this.panel3.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();
                    this.statusStrip1.SendToBack();
                    this.Panel1.Height = 24;
                }
                else
                {
                    this.panel3.Dock = DockStyle.Top;
                    this.Panel1.Dock = DockStyle.None;
                    this._isHoveringVertically = true;
                    this.panel3.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();
                    this.statusStrip1.SendToBack();

                    this.Panel1.BringToFront();
                    this.Panel1.Height = this._openPanelHeight;
                }

                this.ResumeLayout(true);
            }
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.ComboBox2.SelectedIndex = 2;
            else if (e.ZoomWidth)
                this.ComboBox2.SelectedIndex = 3;
            else
                this.ComboBox2.SelectedIndex = 4;

            lock (this._lockObject)
                if (!this._dontUpdateTempData)
                {
                    this._dontUpdateTempData = true;
                    this.TranslateTempDataToZoom(this.tempData);
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    this._dontUpdateTempData = false;
                }
                else
                {
                    if (this.timer4.Enabled)
                        this.timer4.Stop();
                    this.timer4.Start();
                }

            this._dontDoZoom = false;
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? s = ComboBox2.SelectedItem?.ToString();
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(s);
                this.helplineRulerCtrl1.Enabled = true;
                if (this.ComboBox2.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                lock (this._lockObject)
                    if (!this._dontUpdateTempData)
                    {
                        this._dontUpdateTempData = true;
                        this.TranslateTempDataToZoom(this.tempData);
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        this._dontUpdateTempData = false;
                    }
                    else
                    {
                        if (this.timer4.Enabled)
                            this.timer4.Stop();
                        this.timer4.Start();
                    }
            }
        }

        private void CheckBox15_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void TranslatePathAndSeedPoints(int amountX, int amountY)
        {
            if (this.quickExtractingCtrl1.SeedPoints != null && this.quickExtractingCtrl1.SeedPoints.Count > 0)
            {
                for (int i = 0; i <= this.quickExtractingCtrl1.SeedPoints.Count - 1; i++)
                    this.quickExtractingCtrl1.SeedPoints[i] = new PointF(this.quickExtractingCtrl1.SeedPoints[i].X + amountX,
                        this.quickExtractingCtrl1.SeedPoints[i].Y + amountY);

                if (this.quickExtractingCtrl1.TempPath != null && this.quickExtractingCtrl1.TempPath.Count > 0)
                {
                    for (int j = 0; j < this.quickExtractingCtrl1.TempPath.Count; j++)
                    {
                        PointF ptOld = this.quickExtractingCtrl1.TempPath[j];
                        float xOLd = ptOld.X;
                        float yOld = ptOld.Y;

                        this.quickExtractingCtrl1.TempPath[j] = new PointF(xOLd + amountX, yOld + amountY);
                    }
                }

                if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
                {
                    for (int i = 0; i <= this.quickExtractingCtrl1.CurPath.Count - 1; i++)
                    {
                        List<PointF> p = this.quickExtractingCtrl1.CurPath[i];

                        if (p.Count > 0)
                        {
                            for (int j = 0; j <= p.Count - 1; j++)
                            {
                                PointF ptOld2 = p[j];
                                float xOLd2 = ptOld2.X;
                                float yOld2 = ptOld2.Y;

                                p[j] = new PointF(xOLd2 + amountX, yOld2 + amountY);
                            }
                        }
                    }
                }

                if (this._curPathCopy != null && this._curPathCopy.Count > 0)
                {
                    for (int i = 0; i <= this._curPathCopy.Count - 1; i++)
                    {
                        List<PointF> p = this._curPathCopy[i];
                        if (p.Count > 0)
                        {
                            for (int j = 0; j <= p.Count - 1; j++)
                            {
                                PointF ptOld2 = p[j];
                                float xOLd2 = ptOld2.X;
                                float yOld2 = ptOld2.Y;

                                p[j] = new PointF(xOLd2 + amountX, yOld2 + amountY);
                            }
                        }
                    }
                }
            }
        }

        private void TranslateTempDataToZoom(List<Point>? tempData)
        {
            if (tempData != null && tempData.Count > 0)
            {
                if (tempDataZ == null)
                    tempDataZ = new List<RectangleF>();
                this.tempDataZ.Clear();

                int n = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value);

                for (int i = 0; i <= tempData.Count - 1; i += n) // 200
                    if (this.tempData != null)
                        this.tempDataZ.Add(new RectangleF(this.tempData[i].X * this.helplineRulerCtrl1.Zoom - 1,
                            this.tempData[i].Y * this.helplineRulerCtrl1.Zoom - 1, 2, 2));
            }
        }

        private void Button14_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                int ix = this._ix;
                int iy = this._iy;

                if (this.quickExtractingCtrl1.CurPath == null || this.quickExtractingCtrl1.CurPath.Count == 0)
                    return;

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = "train",
                    bmpDataForValueComputation = this.bmpForValueComputation,
                    imgDataPic = this.imgDataPic,
                    MouseClicked = true,
                    Ramps = this.quickExtractingCtrl1.Ramps,
                    SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                    TempPath = this.quickExtractingCtrl1.TempPath,
                    CurPath = this.quickExtractingCtrl1.CurPath,
                    valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                    valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                    valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                    valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                    valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                    valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                    doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                    doR = this.quickExtractingCtrl1.cbR.Checked,
                    doG = this.quickExtractingCtrl1.cbG.Checked,
                    doB = this.quickExtractingCtrl1.cbB.Checked,
                    neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                    useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value)
                };

                this.PrepareBGW(new object[] { 4, new Point(ix, iy), @params });

                this.quickExtractingCtrl1.numValP.Enabled = true;
                this.quickExtractingCtrl1.numVal_I.Enabled = true;
                this.quickExtractingCtrl1.numValO.Enabled = true;
                // Me.NumericUpDown10.Enabled = True
                this.quickExtractingCtrl1.numDisplayEdgeAt.Enabled = true;
                this.quickExtractingCtrl1.numDisplayEdgeAt.Enabled = true;
                this.quickExtractingCtrl1.numDisplayEdgeAt.Enabled = true;
                this.quickExtractingCtrl1.Label12.Enabled = true;
                this.quickExtractingCtrl1.btnTrain.Enabled = false;
                this.quickExtractingCtrl1.btnResetTrain.Enabled = true;
            }
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                int num = System.Convert.ToInt32(o[0]);
                Point pt = (Point)o[1];
                QuickExtractingParameters @params = (QuickExtractingParameters)o[2];

                switch (num)
                {
                    case 1:
                        {
                            this.quickExtractingCtrl1.Alg?.EnumeratePaths(true);

                            @params.CurPath = this.quickExtractingCtrl1.Alg?.CurPath;
                            @params.SeedPoints = this.quickExtractingCtrl1.Alg?.SeedPoints;
                            @params.TempPath = this.quickExtractingCtrl1.Alg?.TempPath;
                            if (this.quickExtractingCtrl1.Alg?.picAlg?.CostMaps != null)
                                @params.Ramps = this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps;

                            e.Result = @params;
                            break;
                        }

                    case 2:
                        {
                            if (this.quickExtractingCtrl1.Alg != null)
                                this.quickExtractingCtrl1.Alg.PrepareForTranslatedPaths(@params.imgDataPic, @params.bmpDataForValueComputation, @params.transX, @params.transY);
                            break;
                        }

                    case 3:
                        {
                            if (this.quickExtractingCtrl1.Alg != null)
                            {
                                this.quickExtractingCtrl1.Alg.MouseClicked = false;

                                if (!this.quickExtractingCtrl1.Alg.isRunning)
                                {
                                    this.quickExtractingCtrl1.Alg.ReInit(pt.X, pt.Y, @params.doScale, @params.neighbors);
                                    this.quickExtractingCtrl1.Alg.EnumeratePaths(true);

                                    @params.CurPath = this.quickExtractingCtrl1.Alg.CurPath;
                                    @params.SeedPoints = this.quickExtractingCtrl1.Alg.SeedPoints;
                                    @params.TempPath = this.quickExtractingCtrl1.Alg.TempPath;
                                    if (this.quickExtractingCtrl1.Alg.picAlg?.CostMaps != null)
                                        @params.Ramps = this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps;

                                    e.Result = @params;
                                }
                            }
                            break;
                        }

                    case 6:
                        {
                            this.quickExtractingCtrl1.Alg?.EnumeratePaths(true, @params.msg);

                            @params.CurPath = this.quickExtractingCtrl1.Alg?.CurPath;
                            @params.SeedPoints = this.quickExtractingCtrl1.Alg?.SeedPoints;
                            @params.TempPath = this.quickExtractingCtrl1.Alg?.TempPath;
                            if (this.quickExtractingCtrl1.Alg?.picAlg?.CostMaps != null)
                                @params.Ramps = this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps;

                            e.Result = @params;
                            break;
                        }

                    case 7:
                        {
                            @params.CurPath = this.quickExtractingCtrl1.Alg?.CurPath;
                            @params.SeedPoints = this.quickExtractingCtrl1.Alg?.SeedPoints;
                            @params.TempPath = this.quickExtractingCtrl1.Alg?.TempPath;
                            if (this.quickExtractingCtrl1.Alg?.picAlg?.CostMaps != null)
                                @params.Ramps = this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps;

                            e.Result = @params;
                            break;
                        }

                    case 71:
                        {
                            if (this.quickExtractingCtrl1.Alg != null)
                            {
                                this.quickExtractingCtrl1.Alg.EnumeratePaths(true, @params.msg);

                                @params.CurPath = this.quickExtractingCtrl1.Alg.CurPath;
                                @params.SeedPoints = this.quickExtractingCtrl1.Alg.SeedPoints;
                                @params.TempPath = this.quickExtractingCtrl1.Alg.TempPath;
                                if (this.quickExtractingCtrl1.Alg.picAlg?.CostMaps != null)
                                    @params.Ramps = this.quickExtractingCtrl1.Alg.picAlg.CostMaps.Ramps;

                                e.Result = @params;
                            }
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Result != null)
            {
                QuickExtractingParameters qe = (QuickExtractingParameters)e.Result;

                if (qe != null)
                {
                    this.quickExtractingCtrl1.Ramps = qe.Ramps;
                    this.quickExtractingCtrl1.SeedPoints = qe.SeedPoints;
                    this.quickExtractingCtrl1.TempPath = qe.TempPath;
                    this.quickExtractingCtrl1.CurPath = qe.CurPath;
                }

                if (this.quickExtractingCtrl1.TempPath != null && this.quickExtractingCtrl1.cbAutoSeeds.Enabled && this.quickExtractingCtrl1.cbAutoSeeds.Checked && this.quickExtractingCtrl1.TempPath.Count > 0 && this._tempLast.X != this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1].X && this._tempLast.Y != this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1].Y)
                {
                    this._tempLast = this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1];
                    if (this.Timer2.Enabled)
                        this.Timer2.Stop();
                    this.Timer2.Start();
                }

                if (this.quickExtractingCtrl1.cbAutoClose.Checked && checkPath())
                    closePath();

                if (this.firstClick)
                    this.firstClick = false;

                if (this.tempData != null)
                    this.tempData.Clear();
                if (this.tempDataZ != null)
                    this.tempDataZ.Clear();
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.Cursor = Cursors.Default;
                _dontDoMouseMove = false;

                this.quickExtractingCtrl1.btnNewPath.Enabled = true;

                //re init the bgw
                this.backgroundWorker1.Dispose();
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorker1.WorkerReportsProgress = true;
                this.backgroundWorker1.WorkerSupportsCancellation = true;
                this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                //this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            }
        }

        private void applyClose()
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0 && this.quickExtractingCtrl1.CurPath[0].Count > 0)
                {
                    int ix = System.Convert.ToInt32(this.quickExtractingCtrl1.CurPath[0][0].X);
                    int iy = System.Convert.ToInt32(this.quickExtractingCtrl1.CurPath[0][0].Y);

                    QuickExtractingParameters @params = new QuickExtractingParameters()
                    {
                        msg = this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex == 0 ? "run" : "run2",
                        bmpDataForValueComputation = this.bmpForValueComputation,
                        imgDataPic = this.imgDataPic,
                        MouseClicked = true,
                        Ramps = this.quickExtractingCtrl1.Ramps,
                        SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                        TempPath = this.quickExtractingCtrl1.TempPath,
                        CurPath = this.quickExtractingCtrl1.CurPath,
                        valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                        valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                        valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                        valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                        valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                        valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                        valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                        valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                        laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                        edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                        doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                        doR = this.quickExtractingCtrl1.cbR.Checked,
                        doG = this.quickExtractingCtrl1.cbG.Checked,
                        doB = this.quickExtractingCtrl1.cbB.Checked,
                        neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                        dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                        useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                        notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value)
                    };

                    if (this.quickExtractingCtrl1.cbAddLine.Checked)
                        this.PrepareBGW(new object[] { 101, new Point(ix, iy), @params });
                    else if (this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex == 0)
                        this.PrepareBGW(new object[] { 1, new Point(ix, iy), @params });
                    else
                        this.PrepareBGW(new object[] { 6, new Point(ix, iy), @params });
                }
            }
        }

        private void closePath()
        {
            if (this.timer1.Enabled)
                this.timer1.Stop();
            if (this.Timer2.Enabled)
                this.Timer2.Stop();
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
            {
                Nullable<PointF> ff = default(PointF?);
                List<PointF> f = this.quickExtractingCtrl1.CurPath[0];
                if (f.Count > 0)
                    ff = f[0];
                if (ff != null)
                {
                    List<PointF> l = this.quickExtractingCtrl1.CurPath[this.quickExtractingCtrl1.CurPath.Count - 1];
                    if (l[l.Count - 1].X != ff.Value.X || l[l.Count - 1].Y != ff.Value.Y)
                        l.Add(ff.Value);
                }
            }

            this.quickExtractingCtrl1.cbRunAlg.Checked = false;
            this.quickExtractingCtrl1.cbRunOnMouseDown.Checked = false;
            this.quickExtractingCtrl1.cbRunAlg.Enabled = false;
            this.quickExtractingCtrl1.cbRunOnMouseDown.Enabled = false;

            this.quickExtractingCtrl1.btnCrop.Enabled = true;
            this.quickExtractingCtrl1.cbCropFromOrig.Enabled = true;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            this._pathClosed = true;
        }

        private void Button13_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                using (frmSelectPen frm = new frmSelectPen())
                {
                    frm.Label1.BackColor = this._fColor;
                    frm.Label2.BackColor = this._zColor;
                    frm.Label3.BackColor = this._highlightColor;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.RadioButton1.Checked)
                        {
                            this.ColorDialog1.Color = this._fColor;
                            if (this.ColorDialog1.ShowDialog() == DialogResult.OK)
                            {
                                this._fColor = this.ColorDialog1.Color;
                                this.quickExtractingCtrl1.btnPenCol.BackColor = this._fColor;
                            }
                        }
                        else if (frm.RadioButton2.Checked)
                        {
                            this.ColorDialog1.Color = this._zColor;
                            if (this.ColorDialog1.ShowDialog() == DialogResult.OK)
                            {
                                this._zColor = this.ColorDialog1.Color;
                                this.quickExtractingCtrl1.btnPenCol.BackColor = this._fColor;
                            }
                        }
                        else if (frm.RadioButton3.Checked)
                        {
                            this.ColorDialog1.Color = this._highlightColor;
                            if (this.ColorDialog1.ShowDialog() == DialogResult.OK)
                            {
                                this._highlightColor = this.ColorDialog1.Color;
                                this.quickExtractingCtrl1.btnPenCol.BackColor = this._fColor;
                            }
                        }
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }
        }

        private void QEAlg_Notify(object sender, NotifyEventArgs e)
        {
            List<Point>? l = e.TempData;

            if (this.tempData == null)
                this.tempData = new List<Point>();

            if (l != null && l.Count > 0)
                this.tempData.AddRange(l);

            lock (this._lockObject)
                if (!this._dontUpdateTempData)
                {
                    this._dontUpdateTempData = true;
                    this.TranslateTempDataToZoom(this.tempData);
                    if (this.tempDataZ != null)
                        this._tempDataZA = this.tempDataZ.ToArray();
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    this._dontUpdateTempData = false;
                }
                else
                {
                    if (this.timer4.Enabled)
                        this.timer4.Stop();
                    this.timer4.Start();
                }
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Run alg to close?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                applyClose();
            else
                closePath();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();

            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (!this._pathClosed)
            {
                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = "update",
                    Ramps = this.quickExtractingCtrl1.Ramps,
                    SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                    TempPath = this.quickExtractingCtrl1.TempPath,
                    CurPath = this.quickExtractingCtrl1.CurPath,
                    doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                    neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                    useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value)
                };

                curPtBU2 = new Point(_ix, _iy);

                if (this.quickExtractingCtrl1.cbAddLine.Checked)
                    this.PrepareBGW(new object[] { 102, new Point(_ix, _iy), @params });
                else if (this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex == 0)
                    this.PrepareBGW(new object[] { 3, new Point(_ix, _iy), @params });
                else
                    this.PrepareBGW(new object[] { 7, new Point(_ix, _iy), @params });
            }
        }

        private void CheckBox5_CheckedChanged(object? sender, EventArgs e)
        {
            this.quickExtractingCtrl1.cbAutoSeeds.Enabled = this.quickExtractingCtrl1.cbRunOnMouseDown.Checked;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            this.Timer2.Stop();
            if (!this._pathClosed)
                CheckAutoSeedPoint();
        }

        private void CheckAutoSeedPoint()
        {
            if (this.quickExtractingCtrl1.TempPath != null && this.quickExtractingCtrl1.TempPath.Count > 0 && this._tempLast.X == this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1].X && this._tempLast.Y == this.quickExtractingCtrl1.TempPath[this.quickExtractingCtrl1.TempPath.Count - 1].Y)
            {
                if (this.helplineRulerCtrl1.Bmp != null)
                {
                    this.quickExtractingCtrl1.AddTempPath(true, true, this._ix, this._iy);
                    if (this.quickExtractingCtrl1.TempPath != null)
                        this.quickExtractingCtrl1.TempPath.Clear();
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    apply();
                }
            }
        }

        private void CheckBox4_CheckedChanged(object? sender, EventArgs e)
        {
            this.quickExtractingCtrl1.cbRunOnMouseDown.Enabled = this.quickExtractingCtrl1.cbRunAlg.Checked;
            this.quickExtractingCtrl1.cbAutoClose.Enabled = this.quickExtractingCtrl1.cbRunAlg.Checked;
            this.quickExtractingCtrl1.cbAutoSeeds.Enabled = this.quickExtractingCtrl1.cbRunOnMouseDown.Checked;
        }

        private void Button3_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                using (frmNewPath frm6 = new frmNewPath())
                {
                    if (frm6.ShowDialog() == DialogResult.OK)
                    {
                        if (frm6.RadioButton1.Checked)
                        {
                            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CheckPaths(this.quickExtractingCtrl1.CurPath) == false)
                            {
                                if (this.quickExtractingCtrl1.PathList == null)
                                    this.quickExtractingCtrl1.PathList = new List<List<List<PointF>>>();
                                List<List<PointF>> pathCopy = this.quickExtractingCtrl1.ClonePath(this.quickExtractingCtrl1.CurPath);
                                this.quickExtractingCtrl1.PathList.Add(pathCopy);
                                _curPathCopy = pathCopy;
                                this.quickExtractingCtrl1.btnSavedPaths.Enabled = this.quickExtractingCtrl1.btnEditPathPoints.Enabled = true;
                            }
                        }
                        if (frm6.RadioButton2.Checked && frm6.CheckBox1.Enabled && frm6.CheckBox1.Checked)
                            this.quickExtractingCtrl1.PathList = new List<List<List<PointF>>>();
                        if (this.quickExtractingCtrl1.Alg != null)
                        {
                            this.quickExtractingCtrl1.Alg.NotifyEdges -= Alg_NotifyEdges;
                            this.quickExtractingCtrl1.Alg.Dispose();
                            this.quickExtractingCtrl1.Alg = null;

                            this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);
                        }
                        ResetVars();
                    }
                }
            }
        }



        private void ResetVars()
        {
            this.firstClick = true;
            this._pathClosed = false;

            this.quickExtractingCtrl1.CurPath = new List<List<PointF>>();
            _ix = -1;
            _iy = -1;
            this.quickExtractingCtrl1.SeedPoints = new List<PointF>();
            this.quickExtractingCtrl1.TempPath = new List<PointF>();
            this.tempData = new List<Point>();

            if (this.tempDataZ != null)
                this.tempDataZ.Clear();

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            this.quickExtractingCtrl1.btnTrain.Enabled = true;
            this.quickExtractingCtrl1.btnResetTrain.Enabled = false;
            this.quickExtractingCtrl1.lblTrainInfo.Text = "not trained";

            this.quickExtractingCtrl1.cbRunAlg.Enabled = true;
            this.quickExtractingCtrl1.cbRunAlg.Checked = true;
            this.quickExtractingCtrl1.btnResetPath.Enabled = false;

            this._curPathCopy = new List<List<PointF>>();
            this._cloneCurPath = true;
        }

        private void Button6_Click(object? sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (this.quickExtractingCtrl1.CurPath != null)
            {
                this.quickExtractingCtrl1.cbRunOnMouseDown.Checked = false;

                this.quickExtractingCtrl1.TempPath = new List<PointF>();

                if (this.quickExtractingCtrl1.CurPath.Count > 0)
                    this.quickExtractingCtrl1.CurPath.RemoveAt(this.quickExtractingCtrl1.CurPath.Count - 1);

                if (this.quickExtractingCtrl1.CurPath.Count == 0)
                    this.quickExtractingCtrl1.CurPath = new List<List<PointF>>();

                if (this.quickExtractingCtrl1.cmbAlgMode.SelectedIndex == 0)
                {
                    if (this.quickExtractingCtrl1.SeedPoints?.Count > 0)
                        this.quickExtractingCtrl1.SeedPoints.RemoveAt(this.quickExtractingCtrl1.SeedPoints.Count - 1);

                    if (this.quickExtractingCtrl1.SeedPoints?.Count == 0)
                    {
                        if (this.quickExtractingCtrl1.Alg != null)
                        {
                            this.quickExtractingCtrl1.Alg.NotifyEdges -= Alg_NotifyEdges;
                            this.quickExtractingCtrl1.Alg.Dispose();
                            this.quickExtractingCtrl1.Alg = null;

                            this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);
                        }
                        this.quickExtractingCtrl1.CurPath = new List<List<PointF>>();
                        this.firstClick = true;
                    }
                }
                else
                {
                    if (this.quickExtractingCtrl1.SeedPoints?.Count > 0)
                    {
                        if (this.quickExtractingCtrl1.SeedPoints.Count == 1)
                            _spBU = this.quickExtractingCtrl1.SeedPoints[0];
                        this.quickExtractingCtrl1.SeedPoints.RemoveAt(this.quickExtractingCtrl1.SeedPoints.Count - 1);
                    }

                    if (this.quickExtractingCtrl1.SeedPoints?.Count == 0 && this.quickExtractingCtrl1.CurPath.Count == 0)
                    {
                        if (this.quickExtractingCtrl1.Alg != null)
                        {
                            this.quickExtractingCtrl1.Alg.NotifyEdges -= Alg_NotifyEdges;
                            this.quickExtractingCtrl1.Alg.Dispose();
                            this.quickExtractingCtrl1.Alg = null;

                            this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);
                        }
                        this.quickExtractingCtrl1.CurPath = new List<List<PointF>>();
                        this.firstClick = true;
                    }
                }

                // refactor into method
                if (this.quickExtractingCtrl1.CurPath.Count > 0)
                {
                    this.quickExtractingCtrl1.SeedPoints = new List<PointF>();

                    if (this.quickExtractingCtrl1.CurPath[0].Count > 0)
                    {
                        PointF pt = this.quickExtractingCtrl1.CurPath[0][0];
                        this.quickExtractingCtrl1.SeedPoints.Add(new PointF(pt.X, pt.Y));
                    }

                    for (int j = 0; j < this.quickExtractingCtrl1.CurPath.Count; j++)
                    {
                        if (this.quickExtractingCtrl1.CurPath[j].Count > 0)
                        {
                            PointF pt = this.quickExtractingCtrl1.CurPath[j][this.quickExtractingCtrl1.CurPath[j].Count - 1];
                            this.quickExtractingCtrl1.SeedPoints.Add(new PointF(pt.X, pt.Y));
                        }
                    }
                }

                if (this.quickExtractingCtrl1.Alg != null)
                    this.quickExtractingCtrl1.Alg.AllScanned = false;

                this.quickExtractingCtrl1.cbRunAlg.Enabled = true;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this._pathClosed = false;
            }
        }

        private void Button7_Click(object? sender, EventArgs e)
        {
            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
            {
                List<PointF> l = this.quickExtractingCtrl1.CurPath[this.quickExtractingCtrl1.CurPath.Count - 1];
                if (l.Count > 0)
                    l.RemoveAt(l.Count - 1);

                if (l.Count == 0)
                    this.quickExtractingCtrl1.CurPath.RemoveAt(this.quickExtractingCtrl1.CurPath.Count - 1);

                if (l.Count > 0 && this.quickExtractingCtrl1.SeedPoints?.Count > 0)
                    this.quickExtractingCtrl1.SeedPoints[this.quickExtractingCtrl1.SeedPoints.Count - 1] = new PointF(l[l.Count - 1].X, l[l.Count - 1].Y);
                else if (this.quickExtractingCtrl1.SeedPoints?.Count > 0)
                    this.quickExtractingCtrl1.SeedPoints.RemoveAt(this.quickExtractingCtrl1.SeedPoints.Count - 1);
            }

            this.quickExtractingCtrl1.cbRunAlg.Enabled = true;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            this._pathClosed = false;
        }

        private void Button15_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                this.quickExtractingCtrl1.numValP.Enabled = false;
                this.quickExtractingCtrl1.numVal_I.Enabled = false;
                this.quickExtractingCtrl1.numValO.Enabled = false;
                // Me.NumericUpDown10.Enabled = False
                this.quickExtractingCtrl1.Label9.Enabled = false;
                this.quickExtractingCtrl1.Label10.Enabled = false;
                this.quickExtractingCtrl1.Label11.Enabled = false;
                this.quickExtractingCtrl1.Label12.Enabled = false;
                this.quickExtractingCtrl1.btnResetTrain.Enabled = false;

                this.quickExtractingCtrl1.btnTrain.Enabled = this.quickExtractingCtrl1.CurPath != null ? this.quickExtractingCtrl1.CurPath.Count > 0 : false;

                int ix = this._ix;
                int iy = this._iy;

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = "train",
                    bmpDataForValueComputation = this.bmpForValueComputation,
                    imgDataPic = this.imgDataPic,
                    MouseClicked = true,
                    Ramps = this.quickExtractingCtrl1.Ramps,
                    SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                    TempPath = this.quickExtractingCtrl1.TempPath,
                    CurPath = this.quickExtractingCtrl1.CurPath,
                    valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                    valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                    valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                    valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                    valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                    valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                    doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                    doR = this.quickExtractingCtrl1.cbR.Checked,
                    doG = this.quickExtractingCtrl1.cbG.Checked,
                    doB = this.quickExtractingCtrl1.cbB.Checked,
                    neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                    useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value)
                };

                this.PrepareBGW(new object[] { 5, new Point(ix, iy), @params });
            }
        }

        private void Button9_Click(object? sender, EventArgs e)
        {
            if (_curPathCopy != null && _curPathCopy.Count > 0)
                this.quickExtractingCtrl1.CurPath = this._curPathCopy;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void frm7_PathsChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                frmSelectCrop f = (frmSelectCrop)sender;
                this._checkedPaths = new List<int>();

                if (f.CheckBox1.Checked)
                    this._checkedPaths.Add(-1);
                if (f.CheckedListBox1.CheckedIndices.Count > 0)
                {
                    for (int i = 0; i <= f.CheckedListBox1.CheckedIndices.Count - 1; i++)
                        this._checkedPaths.Add(f.CheckedListBox1.CheckedIndices[i]);
                }
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }


        private void Button12_Click(object? sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();
            this.quickExtractingCtrl1.cbRunOnMouseDown.Checked = false;
        }

        private void Button25_Click(object? sender, EventArgs e)
        {
            Bitmap? bmp = null;

            if (this._bmpBU != null && this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false && AvailMem.AvailMem.checkAvailRam((this._bmpBU.Width + 200) * (this._bmpBU.Height + 200) * 4L))
            {
                using (frmSelectCrop frm7 = new frmSelectCrop(this.quickExtractingCtrl1.PathList))
                {
                    frm7.PathsChanged += frm7_PathsChanged;
                    if (frm7.ShowDialog() == DialogResult.OK)
                    {
                        //if (_amountX != 0 || _amountY != 0)
                        //{
                        //    if (this.quickExtractingCtrl1.cbCropFromOrig.Checked && MessageBox.Show("Re-translate path(s)?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        //        TranslatePathAndSeedPoints(-_amountX, -_amountY);
                        //}

                        CheckedListBox.CheckedIndexCollection fL = frm7.CheckedListBox1.CheckedIndices;
                        if (frm7.CheckBox1.Checked)
                        {
                            try
                            {
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    if (frm7.RadioButton2.Checked)
                                    {
                                        gp.AddRectangle(new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height));
                                        gp.CloseFigure();
                                        gp.StartFigure();
                                    }
                                    if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CurPath.Count > 0)
                                    {
                                        for (int i = 0; i < this.quickExtractingCtrl1.CurPath.Count; i++)
                                        {
                                            List<PointF> p = this.quickExtractingCtrl1.CurPath[i];
                                            if (p != null && p.Count > 1)
                                                gp.AddLines(p.ToArray());
                                        }
                                    }

                                    gp.CloseFigure();

                                    if (!this.quickExtractingCtrl1.cbCropFromOrig.Checked)
                                    {
                                        bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                                        using (Graphics g = Graphics.FromImage(bmp))
                                        {
                                            if (this.quickExtractingCtrl1.cbSmooth.Checked)
                                                g.SmoothingMode = SmoothingMode.AntiAlias;

                                            using (TextureBrush fBrush = new TextureBrush(this.helplineRulerCtrl1.Bmp))
                                            {
                                                g.FillPath(fBrush, gp);
                                                using (Pen p = new Pen(fBrush, 1))
                                                {
                                                    p.LineJoin = LineJoin.Round;
                                                    g.DrawPath(p, gp);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                                        using (Graphics g = Graphics.FromImage(bmp))
                                        {
                                            if (this.quickExtractingCtrl1.cbSmooth.Checked)
                                                g.SmoothingMode = SmoothingMode.AntiAlias;

                                            using (TextureBrush fBrush = new TextureBrush(this._bmpBU))
                                            {
                                                g.TranslateTransform((bmp.Width - this._bmpBU.Width) / 2, (bmp.Height - this._bmpBU.Height) / 2);
                                                g.FillPath(fBrush, gp);
                                                using (Pen p = new Pen(fBrush, 1))
                                                {
                                                    p.LineJoin = LineJoin.Round;
                                                    g.DrawPath(p, gp);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                if (bmp != null)
                                {
                                    bmp.Dispose();
                                    bmp = null;
                                }
                            }
                        }
                        if (fL.Count > 0 && this.quickExtractingCtrl1.PathList != null)
                        {
                            for (int j = 0; j < fL.Count; j++)
                            {
                                List<List<PointF>> path = this.quickExtractingCtrl1.PathList[fL[j]];

                                try
                                {
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        if (path != null && path.Count > 0)
                                        {
                                            for (int i = 0; i <= path.Count - 1; i++)
                                            {
                                                List<PointF> p = path[i];
                                                if (p != null && p.Count > 1)
                                                    gp.AddLines(p.ToArray());
                                            }
                                        }

                                        gp.CloseFigure();
                                        gp.FillMode = FillMode.Winding;

                                        if (!frm7.RadioButton2.Checked)
                                        {
                                            if (!this.quickExtractingCtrl1.cbCropFromOrig.Checked)
                                            {
                                                if (bmp == null)
                                                    bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                                                using (Graphics g = Graphics.FromImage(bmp))
                                                {
                                                    if (this.quickExtractingCtrl1.cbSmooth.Checked)
                                                        g.SmoothingMode = SmoothingMode.AntiAlias;

                                                    using (TextureBrush fBrush = new TextureBrush(this.helplineRulerCtrl1.Bmp))
                                                    {
                                                        g.FillPath(fBrush, gp);
                                                        using (Pen p = new Pen(fBrush, 1))
                                                        {
                                                            p.LineJoin = LineJoin.Round;
                                                            g.DrawPath(p, gp);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (bmp == null)
                                                    bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                                                using (Graphics g = Graphics.FromImage(bmp))
                                                {
                                                    if (this.quickExtractingCtrl1.cbSmooth.Checked)
                                                        g.SmoothingMode = SmoothingMode.AntiAlias;

                                                    using (TextureBrush fBrush = new TextureBrush(this._bmpBU))
                                                    {
                                                        g.TranslateTransform((bmp.Width - this._bmpBU.Width) / 2, (bmp.Height - this._bmpBU.Height) / 2);
                                                        g.FillPath(fBrush, gp);
                                                        using (Pen p = new Pen(fBrush, 1))
                                                        {
                                                            p.LineJoin = LineJoin.Round;
                                                            g.DrawPath(p, gp);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (!this.quickExtractingCtrl1.cbCropFromOrig.Checked)
                                        {
                                            if (bmp == null)
                                                bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                                            using (Graphics g = Graphics.FromImage(bmp))
                                            {
                                                if (this.quickExtractingCtrl1.cbSmooth.Checked)
                                                    g.SmoothingMode = SmoothingMode.AntiAlias;

                                                using (SolidBrush fBrush = new SolidBrush(Color.Transparent))
                                                {
                                                    g.CompositingMode = CompositingMode.SourceCopy;
                                                    g.FillPath(fBrush, gp);
                                                    using (Pen p = new Pen(fBrush, 1))
                                                    {
                                                        p.LineJoin = LineJoin.Round;
                                                        g.DrawPath(p, gp);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (bmp == null)
                                                bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                                            using (Graphics g = Graphics.FromImage(bmp))
                                            {
                                                if (this.quickExtractingCtrl1.cbSmooth.Checked)
                                                    g.SmoothingMode = SmoothingMode.AntiAlias;

                                                using (SolidBrush fBrush = new SolidBrush(Color.Transparent))
                                                {
                                                    g.CompositingMode = CompositingMode.SourceCopy;
                                                    g.TranslateTransform((bmp.Width - this._bmpBU.Width) / 2, (bmp.Height - this._bmpBU.Height) / 2);
                                                    g.FillPath(fBrush, gp);
                                                    using (Pen p = new Pen(fBrush, 1))
                                                    {
                                                        p.LineJoin = LineJoin.Round;
                                                        g.DrawPath(p, gp);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    if (bmp != null)
                                    {
                                        bmp.Dispose();
                                        bmp = null;
                                    }
                                }
                            }
                        }
                    }

                    frm7.PathsChanged -= frm7_PathsChanged;
                    this._checkedPaths = null;
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");
                    if (this.quickExtractingCtrl1.cbAutoSetResAsNewSrc.Checked)
                    {
                        if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                        {
                            Bitmap? b = new Bitmap(bmp);
                            this.SetBitmap(ref this._bmpBU, ref b);
                            this.SetNewPath();
                        }
                        else
                            MessageBox.Show("Not enough memory");
                    }

                    _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                    this.button4.Enabled = true;

                    CheckRedoButton();

                    this._pic_changed = true;

                    // Me.helplineRulerCtrl1.CalculateZoom()

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    this.CheckBox18.Checked = this.quickExtractingCtrl1.cbAutoSetResAsNewSrc.Checked ? true : false;
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.Cursor = Cursors.Default;

                    //auto save path?
                    if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.CheckPaths(this.quickExtractingCtrl1.CurPath) == false)
                    {
                        if (this.quickExtractingCtrl1.PathList == null)
                            this.quickExtractingCtrl1.PathList = new List<List<List<PointF>>>();
                        List<List<PointF>> pathCopy = this.quickExtractingCtrl1.ClonePath(this.quickExtractingCtrl1.CurPath);
                        this.quickExtractingCtrl1.PathList.Add(pathCopy);
                        _curPathCopy = pathCopy;
                        this.quickExtractingCtrl1.btnSavedPaths.Enabled = this.quickExtractingCtrl1.btnEditPathPoints.Enabled = true;
                    }
                }

                this.Label40.Enabled = true;
                this.Button26.Enabled = true;
            }
        }

        private void SetNewPath()
        {
            if (this.quickExtractingCtrl1.CurPath != null)
                if (this.quickExtractingCtrl1.CheckPaths(this.quickExtractingCtrl1.CurPath) == false)
                {
                    if (this.quickExtractingCtrl1.cbSmooth == null)
                        this.quickExtractingCtrl1.PathList = new List<List<List<PointF>>>();
                    this.quickExtractingCtrl1.PathList?.Add(this.quickExtractingCtrl1.ClonePath(this.quickExtractingCtrl1.CurPath));
                    this.quickExtractingCtrl1.btnSavedPaths.Enabled = true;
                }
            if (this.quickExtractingCtrl1.Alg != null)
            {
                this.quickExtractingCtrl1.Alg.NotifyEdges -= Alg_NotifyEdges;
                this.quickExtractingCtrl1.Alg.Dispose();
                this.quickExtractingCtrl1.Alg = null;

                this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);
            }
            ResetVars();
        }

        private void Button24_Click(object? sender, EventArgs e)
        {
            bool cont = false;
            bool redPts = true;
            double minDist = 1d;
            double epsilon = 1d;
            int selectedPath = -1;
            bool index0IsCurrentPath = true;
            if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.PathList != null)
                using (frmReducePathPoints frm3 = new frmReducePathPoints(this._frm3MinDist, this._frm3Epsilon, this._frm3ReducePoints, this.quickExtractingCtrl1.CurPath, this.quickExtractingCtrl1.PathList))
                {
                    frm3.PathChanged += frm3_PathChanged;
                    if (frm3.ShowDialog() == DialogResult.OK)
                    {
                        redPts = frm3.CheckBox1.Checked;
                        //switch (frm3.ComboBox1.SelectedIndex)
                        //{
                        //    case 0:
                        //        {
                        //            minDist = 0.0;
                        //            break;
                        //        }

                        //    case 1:
                        //        {
                        //            minDist = 0.5;
                        //            break;
                        //        }

                        //    case 2:
                        //        {
                        //            minDist = Math.Sqrt(2.0) / 2.0;
                        //            break;
                        //        }

                        //    case 4:
                        //        {
                        //            minDist = Math.Sqrt(2.0);
                        //            break;
                        //        }

                        //    case 5:
                        //        {
                        //            minDist = 2.0;
                        //            break;
                        //        }

                        //    case 6:
                        //        {
                        //            minDist = 4.0;
                        //            break;
                        //        }

                        //    case 7:
                        //        {
                        //            minDist = 8.0;
                        //            break;
                        //        }
                        //}

                        //switch (frm3.ComboBox2.SelectedIndex)
                        //{
                        //    case 0:
                        //        {
                        //            epsilon = 0.5;
                        //            break;
                        //        }

                        //    case 1:
                        //        {
                        //            epsilon = Math.Sqrt(2.0) / 2.0;
                        //            break;
                        //        }

                        //    case 3:
                        //        {
                        //            epsilon = Math.Sqrt(2.0);
                        //            break;
                        //        }

                        //    case 4:
                        //        {
                        //            epsilon = 2.0;
                        //            break;
                        //        }
                        //}
                        this._frm3MinDist = frm3.ComboBox1.SelectedIndex;
                        this._frm3Epsilon = frm3.ComboBox2.SelectedIndex;
                        this._frm3ReducePoints = redPts;
                        if (frm3.ListBox1.Items.Count > 0)
                        {
                            selectedPath = frm3.ListBox1.SelectedIndex;
                            if (frm3.ListBox1?.Items[0]?.ToString()?.Contains("Current Path") == false)
                                index0IsCurrentPath = false;
                        }
                        cont = true;
                    }
                    frm3.PathChanged -= frm3_PathChanged;
                }

            if (cont)
            {
                using (frmEditPath frm = new frmEditPath(this.helplineRulerCtrl1.Bmp))
                {
                    frm.ToolStripDropDownButton1.DropDownItems[1].PerformClick();
                    if (this.quickExtractingCtrl1.CurPath != null && this.quickExtractingCtrl1.PathList != null)
                    {
                        List<List<PointF>> path = this.quickExtractingCtrl1.CurPath;
                        int numPath = -1;
                        if (selectedPath > 0 || index0IsCurrentPath == false && this.quickExtractingCtrl1.PathList != null)
                        {
                            path = index0IsCurrentPath ? this.quickExtractingCtrl1.PathList[selectedPath - 1] : this.quickExtractingCtrl1.PathList[selectedPath];
                            numPath = index0IsCurrentPath ? selectedPath - 1 : selectedPath;
                        }
                        frm.CurPath = this.quickExtractingCtrl1.ClonePath(path, true, redPts, minDist, epsilon);
                        if (frm.ShowDialog() == DialogResult.OK && frm.CurPath != null)
                        {
                            List<List<PointF>> result = frm.CurPath;
                            if (this._cloneCurPath)
                            {
                                this.quickExtractingCtrl1.CloneCurPath();
                                this._cloneCurPath = false;
                            }

                            if (numPath == -1)
                            {
                                this.quickExtractingCtrl1.CurPath = result;
                                this.quickExtractingCtrl1.EditedPath = this.quickExtractingCtrl1.ClonePath(this.quickExtractingCtrl1.CurPath);
                                this.quickExtractingCtrl1.btnRS.Enabled = true;
                                this.quickExtractingCtrl1.TempPath = new List<PointF>();
                                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                                this.quickExtractingCtrl1.btnResetPath.Enabled = true;
                            }
                            else if (this.quickExtractingCtrl1.PathList != null && result != null)
                                this.quickExtractingCtrl1.PathList[numPath] = result;
                        }
                    }
                }
            }

            this._selectedPath = null;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void frm3_PathChanged(object sender, IntegerEventArgs e)
        {
            int pathNumber = e.Value;
            bool i0IsCP = e.Index0IsCurrentPath;
            if (pathNumber == 0 && i0IsCP && this.quickExtractingCtrl1.CurPath != null)
            {
                this._selectedPath = this.quickExtractingCtrl1.CurPath;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                return;
            }
            if (this.quickExtractingCtrl1.PathList != null && pathNumber > -1 && this.quickExtractingCtrl1.PathList.Count >= pathNumber)
            {
                this._selectedPath = i0IsCP ? this.quickExtractingCtrl1.PathList[pathNumber - 1] : this.quickExtractingCtrl1.PathList[pathNumber];
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumericUpDown1_ValueChanged(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button27_Click(object? sender, EventArgs e)
        {
            if (this.quickExtractingCtrl1.EditedPath != null && this.quickExtractingCtrl1.EditedPath.Count > 0)
                this.quickExtractingCtrl1.CurPath = this.quickExtractingCtrl1.ClonePath(this.quickExtractingCtrl1.EditedPath);
            this.quickExtractingCtrl1.TempPath = new List<PointF>();
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void NumericUpDown2_ValueChanged(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false && this.quickExtractingCtrl1.Alg != null)
                this.quickExtractingCtrl1.Alg.SetVariables(
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                    System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                    System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value));
        }

        private void CheckBox9_CheckedChanged(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false && this.quickExtractingCtrl1.Alg != null)
                this.quickExtractingCtrl1.Alg.SetScaleValues(this.quickExtractingCtrl1.cbScaleValues.Checked);
        }

        private void CheckBox1_CheckedChanged(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false && this.quickExtractingCtrl1.Alg != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();
                this.quickExtractingCtrl1.Alg.SetColorChannels(this.quickExtractingCtrl1.cbR.Checked, this.quickExtractingCtrl1.cbG.Checked, this.quickExtractingCtrl1.cbB.Checked);
                this.Cursor = Cursors.Default;
            }
        }

        private void ComboBox3_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false && this.quickExtractingCtrl1.Alg != null)
                this.quickExtractingCtrl1.Alg.SetNeighbors(this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex);
        }

        private void Button28_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
        }

        private void Button30_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false && this.quickExtractingCtrl1.PathList != null)
            {
                using (frmSavedPaths frm8 = new frmSavedPaths(this.helplineRulerCtrl1.Bmp, this.quickExtractingCtrl1.ClonePath(this.quickExtractingCtrl1.CurPath), CloneList(this.quickExtractingCtrl1.PathList)))
                {
                    if (frm8.ShowDialog() == DialogResult.OK)
                    {
                        this.quickExtractingCtrl1.PathList = frm8.PathList;
                        this.quickExtractingCtrl1.CurPath = frm8.CurPath;

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }
        }

        private List<List<List<PointF>>>? CloneList(List<List<List<PointF>>>? pathList)
        {
            if (pathList != null && pathList.Count > 0)
            {
                List<List<List<PointF>>> lOut = new List<List<List<PointF>>>();
                for (int i = 0; i <= pathList.Count - 1; i++)
                {
                    List<List<PointF>> p = pathList[i];
                    List<List<PointF>> l1 = new List<List<PointF>>();

                    for (int j = 0; j <= p.Count - 1; j++)
                    {
                        List<PointF> q = p[j];
                        List<PointF> l2 = new List<PointF>();
                        l2.AddRange(q);
                        l1.Add(l2);
                    }

                    lOut.Add(l1);
                }

                return lOut;
            }

            return null;
        }

        private void Button31_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                if (this._bmpBU != null && AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 4L))
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, this._bmpBU, this.helplineRulerCtrl1, "Bmp");
                    this.CheckBox18.Checked = true;
                    SetNewPath();
                }
                else
                    MessageBox.Show("Not enough memory");
            }
        }

        private void Button34_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                this.quickExtractingCtrl1.numValL.Value = System.Convert.ToDecimal(0.3);
                this.quickExtractingCtrl1.numLapTh.Value = System.Convert.ToDecimal(184);
                this.quickExtractingCtrl1.numValM.Value = System.Convert.ToDecimal(0.5);
                this.quickExtractingCtrl1.numValG.Value = System.Convert.ToDecimal(0.1);
                this.quickExtractingCtrl1.numEdgeWeight.Value = System.Convert.ToDecimal(0.5);
                this.quickExtractingCtrl1.numValCl.Value = this.quickExtractingCtrl1.numValCl.Minimum;
                this.quickExtractingCtrl1.numValC0l.Value = this.quickExtractingCtrl1.numValC0l.Minimum;

                this.quickExtractingCtrl1.cbScaleValues.Checked = true;

                this.quickExtractingCtrl1.numValP.Value = System.Convert.ToDecimal(0.1);
                this.quickExtractingCtrl1.numVal_I.Value = System.Convert.ToDecimal(0.1);
                this.quickExtractingCtrl1.numValO.Value = System.Convert.ToDecimal(0.1);
            }
        }

        private void CheckBox21_CheckedChanged(object? sender, EventArgs e)
        {
            bool l = !this.quickExtractingCtrl1.cbAddLine.Checked;

            this.quickExtractingCtrl1.numValL.Enabled = l;
            this.quickExtractingCtrl1.numLapTh.Enabled = l;
            this.quickExtractingCtrl1.numValM.Enabled = l;
            this.quickExtractingCtrl1.numValG.Enabled = l;
            this.quickExtractingCtrl1.numEdgeWeight.Enabled = l;
            this.quickExtractingCtrl1.numValCl.Enabled = l;

            this.quickExtractingCtrl1.cbScaleValues.Enabled = l;

            this.quickExtractingCtrl1.numValP.Enabled = l;
            this.quickExtractingCtrl1.numVal_I.Enabled = l;
            this.quickExtractingCtrl1.numValO.Enabled = l;
        }

        private void Button35_Click(object? sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                this.quickExtractingCtrl1.numValL.Value = this.quickExtractingCtrl1.numValL.Minimum;
                this.quickExtractingCtrl1.numLapTh.Value = System.Convert.ToDecimal(184);
                this.quickExtractingCtrl1.numValM.Value = this.quickExtractingCtrl1.numValM.Minimum;
                this.quickExtractingCtrl1.numValG.Value = this.quickExtractingCtrl1.numValG.Minimum;

                this.quickExtractingCtrl1.numValCl.Value = this.quickExtractingCtrl1.numValCl.Minimum;
                this.quickExtractingCtrl1.numValC0l.Value = this.quickExtractingCtrl1.numValC0l.Minimum;

                this.quickExtractingCtrl1.cbScaleValues.Checked = false;

                this.quickExtractingCtrl1.numValP.Value = this.quickExtractingCtrl1.numValP.Minimum;
                this.quickExtractingCtrl1.numVal_I.Value = this.quickExtractingCtrl1.numVal_I.Minimum;
                this.quickExtractingCtrl1.numValO.Value = this.quickExtractingCtrl1.numValO.Minimum;
            }
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            Bitmap? b = null;
            if (this._bmpBU != null && this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                if (AvailMem.AvailMem.checkAvailRam((this._bmpBU.Width + 200) * (this._bmpBU.Height + 200) * 12L))
                    b = new Bitmap(this.helplineRulerCtrl1.Bmp.Width + 200, this.helplineRulerCtrl1.Bmp.Height + 200);
                else
                    return;

                using (Graphics gx = Graphics.FromImage(b))
                {
                    gx.DrawImage(this.helplineRulerCtrl1.Bmp, 100, 100);
                }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.button4.Enabled = true;

                CheckRedoButton();

                this._pic_changed = true;

                double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                if (multiplier >= faktor)
                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                else
                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));
                //this._zoomWidth = false;

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this._dontDoZoom = true;
                this.ComboBox2.SelectedIndex = 4;
                this._dontDoZoom = false;

                this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
                this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;
                this.Label40.Enabled = true;
                this.Button26.Enabled = true;

                if (this.quickExtractingCtrl1.Alg != null)
                    this.quickExtractingCtrl1.Alg.DisposeBmpData();
                this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

                TranslatePathAndSeedPoints(100, 100);

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = "translate",
                    bmpDataForValueComputation = this.bmpForValueComputation,
                    imgDataPic = this.imgDataPic,
                    MouseClicked = true,
                    Ramps = this.quickExtractingCtrl1.Ramps,
                    SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                    TempPath = this.quickExtractingCtrl1.TempPath,
                    CurPath = this.quickExtractingCtrl1.CurPath,
                    valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                    valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                    valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                    valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                    valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                    valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                    doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                    doR = this.quickExtractingCtrl1.cbR.Checked,
                    doG = this.quickExtractingCtrl1.cbG.Checked,
                    doB = this.quickExtractingCtrl1.cbB.Checked,
                    neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                    useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value),
                    amountX = 100,
                    amountY = 100
                };

                this.PrepareBGW(new object[] { 2, new Point(_ix, _iy), @params });
            }
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            Bitmap? b = null;
            if (this._bmpBU != null && this.helplineRulerCtrl1.Bmp != null && this.backgroundWorker1.IsBusy == false)
            {
                if (AvailMem.AvailMem.checkAvailRam((this._bmpBU.Width) * (this._bmpBU.Height) * 12L))
                    b = new Bitmap(this._bmpBU.Width, this._bmpBU.Height);
                else
                    return;

                using (Graphics gx = Graphics.FromImage(b))
                {
                    gx.DrawImage(this.helplineRulerCtrl1.Bmp, -100, -100);
                }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.button4.Enabled = true;

                CheckRedoButton();

                this._pic_changed = true;

                double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                if (multiplier >= faktor)
                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                else
                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));
                //this._zoomWidth = false;

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this._dontDoZoom = true;
                this.ComboBox2.SelectedIndex = 4;
                this._dontDoZoom = false;

                this._bmpWidth = this.helplineRulerCtrl1.Bmp.Width;
                this._bmpHeight = this.helplineRulerCtrl1.Bmp.Height;

                if (this.quickExtractingCtrl1.Alg != null)
                    this.quickExtractingCtrl1.Alg.DisposeBmpData();
                this.imgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                this.bmpForValueComputation = new Bitmap(this.imgDataPic.Width, this.imgDataPic.Height);

                TranslatePathAndSeedPoints(-100, -100);

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = "translate",
                    bmpDataForValueComputation = this.bmpForValueComputation,
                    imgDataPic = this.imgDataPic,
                    MouseClicked = true,
                    Ramps = this.quickExtractingCtrl1.Ramps,
                    SeedPoints = this.quickExtractingCtrl1.SeedPoints,
                    TempPath = this.quickExtractingCtrl1.TempPath,
                    CurPath = this.quickExtractingCtrl1.CurPath,
                    valL = System.Convert.ToDouble(this.quickExtractingCtrl1.numValL.Value),
                    valM = System.Convert.ToDouble(this.quickExtractingCtrl1.numValM.Value),
                    valG = System.Convert.ToDouble(this.quickExtractingCtrl1.numValG.Value),
                    valP = System.Convert.ToDouble(this.quickExtractingCtrl1.numValP.Value),
                    valI = System.Convert.ToDouble(this.quickExtractingCtrl1.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.quickExtractingCtrl1.numValO.Value),
                    valCl = System.Convert.ToDouble(this.quickExtractingCtrl1.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.quickExtractingCtrl1.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.quickExtractingCtrl1.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.quickExtractingCtrl1.numEdgeWeight.Value),
                    doScale = this.quickExtractingCtrl1.cbScaleValues.Checked,
                    doR = this.quickExtractingCtrl1.cbR.Checked,
                    doG = this.quickExtractingCtrl1.cbG.Checked,
                    doB = this.quickExtractingCtrl1.cbB.Checked,
                    neighbors = this.quickExtractingCtrl1.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.quickExtractingCtrl1.numTrainDist.Value),
                    useCostMap = this.quickExtractingCtrl1.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.quickExtractingCtrl1.numDisplayEdgeAt.Value),
                    amountX = -100,
                    amountY = -100
                };

                this.PrepareBGW(new object[] { 2, new Point(_ix, _iy), @params });
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            this.timer4.Stop();

            this._dontUpdateTempData = true;
            this.TranslateTempDataToZoom(this.tempData);
            if (this.tempDataZ != null)
                this._tempDataZA = this.tempDataZ.ToArray();
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
            this._dontUpdateTempData = false;
        }
    }
}
