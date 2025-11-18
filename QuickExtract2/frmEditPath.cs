using AvailMem;
using ChainCodeFinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace QuickExtract2
{
    public partial class frmEditPath : Form
    {
        private bool _cloneCurPath = true;
        private List<List<PointF>>? _curPathCopy;
        private List<List<PointF>>? CurPathZ;
        // Private _dontDrawPath As Boolean
        private bool _dontDoZoom;
        //private bool _zoomWidth;
        private int _bmpWidth;
        private int _bmpHeight;
        private Bitmap? _bmpBU; // Wohl nicht nötig
        private int _ix;
        private int _iy;
        public List<List<PointF>>? CurPath { get; set; }
        private Color _fColor = Color.Yellow;
        private Color _zColor = Color.Orange;
        private int _pointCount;
        private List<PointF>? _listCorrespondingPts;
        private int _selctedIndex;
        private RectangleF currentPointRegion;
        private List<PointF>? _widenedPoints;
        private List<PointF>? _widenedPointsZ;
        private bool _pointFound;
        private bool _drawSinglePoint;
        private int _borderSum;
        private bool _frm4RB;
        private float _curZoom;
        private bool _crRemoveRange;
        private int _selctedStPtIndex = -1;
        private int _selctedEPtIndex = -1;
        private frmRemoveRange? _frmRR;

        public Bitmap FBitmap
        {
            get
            {
                return this.HelplineRulerCtrl1.Bmp;
            }
        }

        public frmEditPath(Bitmap bmp)
        {
            InitializeComponent();

            HelperFunctions.HelperFunctions.SetFormSizeBig(this);
            this.CenterToScreen();

            // Me.HelplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.HelplineRulerCtrl1.Bmp = (Bitmap)bmp.Clone();
                _bmpBU = (Bitmap)bmp.Clone();
            }
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }

            this._bmpWidth = this.HelplineRulerCtrl1.Bmp.Width;
            this._bmpHeight = this.HelplineRulerCtrl1.Bmp.Height;

            double faktor = System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width));
            else
                this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height));
            //this._zoomWidth = false;

            this.HelplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Height * this.HelplineRulerCtrl1.Zoom));
            this.HelplineRulerCtrl1.MakeBitmap(this.HelplineRulerCtrl1.Bmp);

            this.HelplineRulerCtrl1.AddDefaultHelplines();
            this.HelplineRulerCtrl1.ResetAllHelpLineLabelsColor();

            this.HelplineRulerCtrl1.dbPanel1.DragOver += bitmapPanel1_DragOver;
            this.HelplineRulerCtrl1.dbPanel1.DragDrop += bitmapPanel1_DragDrop;

            this.HelplineRulerCtrl1.dbPanel1.MouseDown += Helplinerulerctrl1_MouseDown;
            this.HelplineRulerCtrl1.dbPanel1.MouseMove += Helplinerulerctrl1_MouseMove;
            this.HelplineRulerCtrl1.dbPanel1.MouseUp += Helplinerulerctrl1_MouseUp;
            this.HelplineRulerCtrl1.PostPaint += Helplinerulerctrl1_Paint;

            this._dontDoZoom = true;
            this.ComboBox2.SelectedIndex = 4;
            this._dontDoZoom = false;

            // Me._zColor = GetCorrespondingColor(Me._fColor)

            this.CurPath = new List<List<PointF>>();
        }

        private void Helplinerulerctrl1_Paint(object sender, PaintEventArgs e)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                int x = this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                int y = this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                if (CurPathZ != null && CurPathZ.Count > 0)
                {
                    for (int i = 0; i < CurPathZ.Count; i++)
                    {
                        List<PointF> p = CurPathZ[i];
                        if (p != null && p.Count > 1)
                            gp.AddLines(p.ToArray());
                    }
                    e.Graphics.TranslateTransform(x, y);
                    if (gp.PointCount > 2 && this._widenedPointsZ != null)
                    {
                        if (this._selctedIndex < this._widenedPointsZ.Count)
                        {
                            bool rev = false;
                            PointF[] p = new PointF[gp.PathPoints.Length];
                            gp.PathPoints.CopyTo(p, 0);

                            if ((!CheckUmlauf(p)))
                                rev = true;
                            if (this.CheckBox2.Checked)
                                rev = !rev;
                            if (rev && CurPath != null)
                            {
                                if (this._crRemoveRange && this._selctedStPtIndex >= 0 && this._selctedEPtIndex >= 0 && this._selctedStPtIndex < CurPath[0].Count && this._selctedEPtIndex < CurPath[0].Count)
                                {
                                    using (GraphicsPath gP2 = new GraphicsPath())
                                    {
                                        List<PointF> pts = CurPathZ[0];
                                        List<PointF> pathPart = new List<PointF>();
                                        int st = this._selctedStPtIndex;
                                        int ed = this._selctedEPtIndex;
                                        int cnt = pts.Count;
                                        if (pts != null && pts.Count > 1)
                                        {
                                            if (this._selctedStPtIndex > this._selctedEPtIndex)
                                                ed += cnt;

                                            for (int i = st; i <= ed; i++)
                                                pathPart.Add(pts[i % cnt]);

                                            if (pathPart.Count > 1)
                                                gP2.AddLines(pathPart.ToArray());
                                        }

                                        float wh = System.Convert.ToSingle(this.NumericUpDown3.Value);
                                        using (Pen pen = new Pen(new SolidBrush(Color.OrangeRed), wh * 4))
                                        {
                                            if (gP2.PointCount > 1)
                                                e.Graphics.DrawPath(pen, gP2);
                                        }
                                    }

                                    PointF spt = this._widenedPointsZ[this._selctedStPtIndex];
                                    e.Graphics.DrawLine(Pens.White, p[this._selctedStPtIndex], spt);
                                    e.Graphics.FillEllipse(Brushes.Yellow, spt.X - 12, spt.Y - 12, 24, 24);
                                    e.Graphics.DrawEllipse(Pens.Black, spt.X - 12, spt.Y - 12, 24, 24);

                                    PointF ept = this._widenedPointsZ[this._selctedEPtIndex];
                                    e.Graphics.DrawLine(Pens.White, p[this._selctedEPtIndex], ept);
                                    e.Graphics.FillEllipse(Brushes.Orange, ept.X - 12, ept.Y - 12, 24, 24);
                                    e.Graphics.DrawEllipse(Pens.Black, ept.X - 12, ept.Y - 12, 24, 24);
                                }
                                else
                                {
                                    if (this._drawSinglePoint)
                                    {
                                        PointF pt = this._widenedPointsZ[this._selctedIndex];
                                        e.Graphics.DrawLine(Pens.White, p[this._selctedIndex], pt);
                                        e.Graphics.FillEllipse(Brushes.Red, pt.X - 8, pt.Y - 8, 16, 16);
                                        e.Graphics.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                                    }
                                    else
                                    {
                                        for (int i = 0; i <= p.Length - 1; i++)
                                        {
                                            PointF pt = this._widenedPointsZ[i];
                                            e.Graphics.DrawLine(Pens.White, p[i], pt);

                                            if (i == this._selctedIndex)
                                            {
                                                e.Graphics.FillEllipse(Brushes.Red, pt.X - 8, pt.Y - 8, 16, 16);
                                                e.Graphics.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                                            }
                                            else
                                            {
                                                e.Graphics.FillEllipse(Brushes.LightBlue, pt.X - 8, pt.Y - 8, 16, 16);
                                                e.Graphics.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (CurPath != null && this._crRemoveRange && this._selctedStPtIndex >= 0 && this._selctedEPtIndex >= 0 && this._selctedStPtIndex < CurPath[0].Count && this._selctedEPtIndex < CurPath[0].Count)
                                {
                                    using (GraphicsPath gP2 = new GraphicsPath())
                                    {
                                        List<PointF> pts = CurPathZ[0];
                                        List<PointF> pathPart = new List<PointF>();
                                        int st = this._selctedStPtIndex;
                                        int ed = this._selctedEPtIndex;
                                        int cnt = pts.Count;
                                        if (pts != null && pts.Count > 1)
                                        {
                                            if (this._selctedStPtIndex > this._selctedEPtIndex)
                                                ed += cnt;

                                            for (int i = st; i <= ed; i++)
                                                pathPart.Add(pts[i % cnt]);

                                            if (pathPart.Count > 1)
                                                gP2.AddLines(pathPart.ToArray());
                                        }

                                        float wh = System.Convert.ToSingle(this.NumericUpDown3.Value);
                                        using (Pen pen = new Pen(new SolidBrush(Color.OrangeRed), wh * 4))
                                        {
                                            if (gP2.PointCount > 1)
                                                e.Graphics.DrawPath(pen, gP2);
                                        }
                                    }

                                    PointF spt = this._widenedPointsZ[this._selctedStPtIndex];
                                    e.Graphics.DrawLine(Pens.White, p[this._selctedStPtIndex], spt);
                                    e.Graphics.FillEllipse(Brushes.Yellow, spt.X - 12, spt.Y - 12, 24, 24);
                                    e.Graphics.DrawEllipse(Pens.Black, spt.X - 12, spt.Y - 12, 24, 24);

                                    PointF ept = this._widenedPointsZ[this._selctedEPtIndex];
                                    e.Graphics.DrawLine(Pens.White, p[this._selctedEPtIndex], ept);
                                    e.Graphics.FillEllipse(Brushes.Orange, ept.X - 12, ept.Y - 12, 24, 24);
                                    e.Graphics.DrawEllipse(Pens.Black, ept.X - 12, ept.Y - 12, 24, 24);
                                }
                                else
                                {
                                    if (this._drawSinglePoint)
                                    {
                                        PointF pt = this._widenedPointsZ[this._selctedIndex];
                                        e.Graphics.DrawLine(Pens.White, p[this._selctedIndex], pt);
                                        e.Graphics.FillEllipse(Brushes.Red, pt.X - 8, pt.Y - 8, 16, 16);
                                        e.Graphics.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                                    }
                                    else
                                    {
                                        for (int i = 0; i <= p.Length - 1; i++)
                                        {
                                            PointF pt = this._widenedPointsZ[i];
                                            e.Graphics.DrawLine(Pens.White, p[i], pt);

                                            if (i == this._selctedIndex)
                                            {
                                                e.Graphics.FillEllipse(Brushes.Red, pt.X - 8, pt.Y - 8, 16, 16);
                                                e.Graphics.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                                            }
                                            else
                                            {
                                                e.Graphics.FillEllipse(Brushes.LightBlue, pt.X - 8, pt.Y - 8, 16, 16);
                                                e.Graphics.DrawEllipse(Pens.Black, pt.X - 8, pt.Y - 8, 16, 16);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    float w = System.Convert.ToSingle(this.NumericUpDown3.Value);
                    using (Pen pen = new Pen(new SolidBrush(this._fColor), w))
                    {
                        e.Graphics.DrawPath(pen, gp);
                    }
                }
            }
        }

        private void Helplinerulerctrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            // If Me._pointFound Then
            // If Me.CheckBox1.Checked Then
            // Me.ListBox1.SelectedIndex = Me._selctedIndex
            // End If
            // End If
            if (this.HelplineRulerCtrl1.Zoom != this._curZoom)
            {
                TranslatePathToZoom(this.CurPath);
                PrepareWorker();
                this.BackgroundWorker1.RunWorkerAsync();
            }

            this._pointFound = false;
        }

        private void Helplinerulerctrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null)
            {
                // && !this._dontDoMove

                int ix = System.Convert.ToInt32((e.X - this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.HelplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.HelplineRulerCtrl1.Zoom);

                int offsetX = ix - _ix;
                int offsetY = iy - _iy;

                if (ix != _ix || iy != _iy)
                {
                    _ix = ix;
                    _iy = iy;

                    if (_ix < 0)
                        _ix = 0;

                    if (_iy < 0)
                        _iy = 0;

                    if (_ix > this.HelplineRulerCtrl1.Bmp.Width - 1)
                        _ix = this.HelplineRulerCtrl1.Bmp.Width - 1;

                    if (_iy > this.HelplineRulerCtrl1.Bmp.Height - 1)
                        _iy = this.HelplineRulerCtrl1.Bmp.Height - 1;

                    Bitmap b = (Bitmap)this.HelplineRulerCtrl1.Bmp;
                    Color c = b.GetPixel(_ix, _iy);

                    toolStripStatusLabel1.Text = "x: " + _ix.ToString() + ", y: " + _iy.ToString() + " - " + "GrayValue (all channels): " + System.Convert.ToInt32(Math.Min(System.Convert.ToDouble(c.B) * 0.11 + System.Convert.ToDouble(c.G) * 0.59 + System.Convert.ToDouble(c.R) * 0.3, 255)).ToString() + " - RGB: " + c.R.ToString() + ";" + c.G.ToString() + ";" + c.B.ToString();

                    this.ToolStripStatusLabel2.BackColor = c;
                }

                if (this._pointFound && this.CurPath != null && this._listCorrespondingPts != null)
                {
                    PointF p = this.CurPath[0][this._selctedIndex];
                    p = new PointF(p.X + offsetX, p.Y + offsetY);
                    this.CurPath[0][this._selctedIndex] = p;
                    this.ListBox1.Items[this._selctedIndex] = "X: " + p.X + "; Y: " + p.Y;
                    this._listCorrespondingPts[this._selctedIndex] = p;
                    CPZTranslatePathPointToZoom(this._selctedIndex, p);
                    GetWidenedPoint(this._selctedIndex, new PointF(offsetX, offsetY));
                    this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                    this.NumericUpDown1.Value = System.Convert.ToDecimal(p.X);
                    this.NumericUpDown2.Value = System.Convert.ToDecimal(p.Y);

                    this.Button9.Enabled = true;

                    _ix = ix;
                    _iy = iy;
                }
            }
        }

        private void Helplinerulerctrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this._listCorrespondingPts != null && this.HelplineRulerCtrl1.Bmp != null && this._widenedPoints != null && this._widenedPoints.Count > 0 && this._widenedPoints.Count == this._listCorrespondingPts.Count)
            {
                _ix = System.Convert.ToInt32((e.X - this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.HelplineRulerCtrl1.Zoom);
                _iy = System.Convert.ToInt32((e.Y - this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.HelplineRulerCtrl1.Zoom);

                bool found = false;

                if (this.currentPointRegion.Contains(_ix, _iy))
                {
                    found = true;
                    if (this.CheckBox1.Checked)
                        this.ListBox1.SelectedIndex = this._selctedIndex;
                }

                if (!found)
                {
                    for (int i = 0; i <= this._widenedPoints.Count - 1; i++)
                    {
                        RectangleF r = new RectangleF(this._widenedPoints[i].X - (float)(8 / (double)this.HelplineRulerCtrl1.Zoom), this._widenedPoints[i].Y - (float)(8 / (double)this.HelplineRulerCtrl1.Zoom),
                            (float)(16 / (double)this.HelplineRulerCtrl1.Zoom), (float)(16 / (double)this.HelplineRulerCtrl1.Zoom));
                        if (r.Contains(_ix, _iy))
                        {
                            found = true;
                            if (this.CheckBox1.Checked)
                                this.ListBox1.SelectedIndex = i;
                            this._selctedIndex = i;
                            break;
                        }
                    }
                }

                if (found)
                {
                    _pointFound = true;
                    this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                }
                else if (this.CheckBox1.Checked && this._listCorrespondingPts != null && this._listCorrespondingPts.Count == this._widenedPoints.Count)
                {
                    double minDist = double.MaxValue;
                    int j = -1;
                    for (int i = 0; i <= this._listCorrespondingPts.Count - 1; i++)
                    {
                        PointF p = this._listCorrespondingPts[i];
                        double dx = _ix - p.X;
                        double dy = _iy - p.Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            j = i;
                        }
                    }
                    if (j > -1)
                    {
                        if (this.CheckBox1.Checked)
                            this.ListBox1.SelectedIndex = j;
                        this._selctedIndex = j;
                        this.currentPointRegion = new RectangleF(this._widenedPoints[j].X - (float)(8 / (double)this.HelplineRulerCtrl1.Zoom), this._widenedPoints[j].Y - (float)(8 / (double)this.HelplineRulerCtrl1.Zoom),
                            (float)(16 / (double)this.HelplineRulerCtrl1.Zoom), (float)(16 / (double)this.HelplineRulerCtrl1.Zoom));
                        _pointFound = true;
                        this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }

                if (!_pointFound)
                    this.currentPointRegion = new RectangleF(-1, -1, 0, 0);
            }
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
                                        this.SetBitmap(this.HelplineRulerCtrl1.Bmp, b1, this.HelplineRulerCtrl1, "Bmp");
                                        b2 = (Bitmap)img.Clone();
                                        this.SetBitmap(ref this._bmpBU, ref b2);
                                    }
                                    else
                                        throw new Exception();
                                }

                                double faktor = System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Height);
                                double multiplier = System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height);
                                if (multiplier >= faktor)
                                    this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width));
                                else
                                    this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height));
                                //this._zoomWidth = false;

                                this.HelplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Height * this.HelplineRulerCtrl1.Zoom));
                                this.HelplineRulerCtrl1.MakeBitmap(this.HelplineRulerCtrl1.Bmp);
                                this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                                this._dontDoZoom = true;
                                this.ComboBox2.SelectedIndex = 4;
                                this._dontDoZoom = false;

                                this.Text = files[0] + " - frmQuickExtract";

                                this._bmpWidth = this.HelplineRulerCtrl1.Bmp.Width;
                                this._bmpHeight = this.HelplineRulerCtrl1.Bmp.Height;
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

        private void Button16_Click(object sender, EventArgs e)
        {
            bool pc = this.CheckBox16.Checked;
            if (pc)
                Button22.PerformClick();

            List<List<PointF>> pathCopy = new List<List<PointF>>();

            if (this._curPathCopy != null && this.CheckBox10.Checked)
            {
                this.CurPath = new List<List<PointF>>();
                for (int i = 0; i <= this._curPathCopy.Count() - 1; i++)
                {
                    List<PointF> p = this._curPathCopy[i];
                    List<PointF> q = new List<PointF>();
                    q.AddRange(p);
                    this.CurPath.Add(q);
                }
            }

            if (_cloneCurPath)
            {
                if (CurPath != null && CurPath.Count > 0)
                {
                    for (int i = 0; i <= CurPath.Count - 1; i++)
                    {
                        List<PointF> p = CurPath[i];

                        PointF[] l = new PointF[p.Count - 1 + 1];
                        p.CopyTo(l);
                        pathCopy.Add(l.ToList());
                    }
                }
            }

            using (GraphicsPath gp = GetPath(this.CheckBox13.Checked, System.Convert.ToDouble(this.NumericUpDown11.Value), false, this.CheckBox11.Checked, this.CheckBox15.Checked))
            {
                if (gp.PointCount > 1)
                {
                    PointF[] p = new PointF[gp.PathPoints.Length];
                    gp.PathPoints.CopyTo(p, 0);
                    PointF[] pts = p;

                    this.CurPath = new List<List<PointF>>();
                    this.CurPath.Add(pts.ToList());
                }
                else
                    MessageBox.Show("Path is empty");
            }

            if (_cloneCurPath)
            {
                _curPathCopy = pathCopy;
                _cloneCurPath = false;
                this.Button9.Enabled = true;
            }

            TranslatePathToZoom(this.CurPath);
            PrepareWorker();
            this.BackgroundWorker1.RunWorkerAsync();
            // GetWidenedPoints()
            SetUpListBox();
            this.Button9.Enabled = true;
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private GraphicsPath GetPath(bool smoothen, double epsilon, bool addcurves, bool remColinearity, bool getOutliers)
        {
            GraphicsPath fPath = new GraphicsPath();
            List<PointF> lList2 = new List<PointF>();
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = false;

            if (this.CurPath != null)
                for (int i = 0; i <= this.CurPath.Count - 1; i++)
                {
                    List<PointF> fList = new List<PointF>();
                    fList.AddRange(this.CurPath[i]);

                    fList = CleanList(fList, System.Convert.ToDouble(this.NumericUpDown12.Value));

                    List<PointF> lList = new List<PointF>();
                    if (fList.Count > 1)
                    {
                        lList.AddRange(fList);

                        if (remColinearity)
                            lList = cf.RemoveColinearity(lList, true);
                        if (smoothen)
                            lList = cf.ApproximateLines(lList, epsilon);
                    }
                    lList2.AddRange(lList);
                }

            // second pass, test
            if (lList2.Count > 1)
            {
                if (remColinearity)
                    lList2 = cf.RemoveColinearity(lList2, true);
                if (smoothen)
                    lList2 = cf.ApproximateLines(lList2, epsilon);

                fPath.Reset();
                if (addcurves)
                    fPath.AddCurve(lList2.ToArray());
                else
                    fPath.AddLines(lList2.ToArray());
            }

            // test
            if (getOutliers)
            {
                int windowSize = System.Convert.ToInt32(this.NumericUpDown13.Value);
                double eps = System.Convert.ToDouble(this.NumericUpDown15.Value);
                double epsMin = System.Convert.ToDouble(this.NumericUpDown14.Value);

                List<PointDataPath> data = GetOutlierData(lList2, windowSize);

                // data = data.OrderBy(Function(f) f.Index).ToList()
                IEnumerable<IGrouping<int, PointDataPath>> groupedData = data.GroupBy(a => a.Index);

                List<int> outliers = new List<int>();
                foreach (IGrouping<int, PointDataPath> group in groupedData)
                {
                    double maxChange = MeasureMaxChange(group);

                    if (IsOutlier(maxChange, eps, epsMin))
                        outliers.Add(group.ToArray()[0].Index);
                }

                outliers = outliers.OrderByDescending(a => a).ToList();

                MessageBox.Show(outliers.Count.ToString());

                for (int i = 0; i <= outliers.Count - 1; i++)
                    lList2.RemoveAt(outliers[i]);

                fPath.Reset();
                if (addcurves)
                    fPath.AddCurve(lList2.ToArray());
                else
                    fPath.AddLines(lList2.ToArray());
            }

            return fPath;
        }

        private bool IsOutlier(double maxChange, double epsilon, double epsMin)
        {
            if (maxChange < epsilon && maxChange >= epsMin)
                return true;

            return false;
        }

        private double MeasureMaxChange(IGrouping<int, PointDataPath> group)
        {
            double avg = group.Average(a => a.Distance);
            double maxChange = 0;

            for (int i = 0; i <= group.Count() - 1; i++)
            {
                double ch = Math.Abs(group.ToArray()[i].Distance - avg);

                if (ch > maxChange)
                    maxChange = ch;
            }

            return maxChange;
        }

        private List<PointDataPath> GetOutlierData(List<PointF> lList, int windowSize)
        {
            List<PointDataPath> lOut = new List<PointDataPath>();

            int w = Math.Min(lList.Count, windowSize);

            if (lList.Count > 1)
            {
                for (int i = 0; i <= lList.Count - 1; i++)
                {
                    // Get current Set to explore
                    PointF[] curData = new PointF[w - 1 + 1];
                    FillCurData(curData, lList, i, w);

                    // get the maxDistPoint and its index
                    GetMaxDistPoint(curData, lOut, i, lList.Count);
                }
            }

            return lOut;
        }

        private void GetMaxDistPoint(PointF[] curData, List<PointDataPath> lOut, int i, int cnt)
        {
            int strt = 0;
            int end = curData.Length - 1;

            PointDataPath[] distances = new PointDataPath[curData.Length - 3 + 1];

            double dx = 0;
            double dy = 0;

            double distPQ = 0;
            double distT = 0;

            PointF p = curData[0];
            PointF q = curData[curData.Length - 1];

            dx = q.X - p.X;
            dy = q.Y - p.Y;

            distPQ = Math.Sqrt(dx * dx + dy * dy);

            double dd = dy / dx;

            for (int j = strt + 1; j <= end - 1; j++)
            {
                PointF t = curData[j];

                double d = q.Y - (q.X * dd);
                double vy = ((t.X * dd) + d);
                double A = (t.Y - vy) * dx;

                distT = System.Convert.ToDouble(Math.Abs(A)) / distPQ;

                if (dx == 0)
                {
                    if (t.X != p.X)
                        distT = Math.Abs(p.X - t.X);
                    else
                        distT = 0;
                }

                int l = i + j;
                if (l > cnt - 1)
                    l -= cnt;
                distances[j - (strt + 1)] = new PointDataPath() { Distance = distT, Index = l };
            }

            if (distances.Length > 0)
            {
                PointDataPath result = distances.OrderBy(f => f.Distance).ToArray()[distances.Length - 1];
                lOut.Add(result);
            }
        }

        private void FillCurData(PointF[] curData, List<PointF> lList, int i, int w)
        {
            for (int j = i; j <= i + w - 1; j++)
            {
                if (j < lList.Count)
                    curData[j - i] = lList[j];
                else
                    curData[j - i] = lList[j - lList.Count];
            }
        }

        private List<PointF> CleanList(List<PointF> fList, double minDist)
        {
            if (fList.Count > 1)
            {
                List<PointF> l = new List<PointF>();
                PointF curPt = fList[0];
                l.Add(curPt);
                double dist = 0.0;
                int i = 1;
                int j = 1;

                if (minDist > 0)
                {
                    while (j < fList.Count)
                    {
                        while (dist < minDist && i + j < fList.Count)
                        {
                            double x = Math.Abs(curPt.X - fList[j + i].X);
                            double y = Math.Abs(curPt.Y - fList[j + i].Y);
                            dist += Math.Sqrt(x * x + y * y);
                            curPt = fList[j];
                            i += 1;
                        }
                        l.Add(curPt);
                        dist = 0.0;

                        j += Math.Max(i, 1);
                        i = 1;
                    }

                    if (l[l.Count - 1].X != fList[fList.Count - 1].X || l[l.Count - 1].Y != fList[fList.Count - 1].Y)
                        l.Add(fList[fList.Count - 1]);

                    return l;
                }
                else
                    return fList;
            }
            else
                return fList;
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.Refresh();
            if (_curPathCopy != null && _curPathCopy.Count > 0)
                this.CurPath = CopyPath(this._curPathCopy);

            TranslatePathToZoom(this.CurPath);
            PrepareWorker();
            this.BackgroundWorker1.RunWorkerAsync();
            // GetWidenedPoints()
            SetUpListBox();
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            this.Cursor = Cursors.Default;
        }

        private void TranslatePathToZoom(List<List<PointF>>? curPath)
        {
            if (this.CurPath != null && curPath != null && curPath.Count > 0)
            {
                if (this.CurPath[0].Count > 600)
                    this.CheckBox18.Checked = false;
                else
                    this.CheckBox18.Checked = true;
                if (CurPathZ == null)
                    CurPathZ = new List<List<PointF>>();
                this.CurPathZ.Clear();

                for (int i = 0; i <= curPath.Count - 1; i++)
                {
                    List<PointF> p = curPath[i];
                    List<PointF> l = new List<PointF>();

                    for (int j = 0; j <= p.Count - 1; j++)
                        l.Add(new PointF(p[j].X * this.HelplineRulerCtrl1.Zoom, p[j].Y * this.HelplineRulerCtrl1.Zoom));

                    this.CurPathZ.Add(l);
                }
            }
        }

        private object TempPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private void Button21_Click(object sender, EventArgs e)
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();
            List<PointF> pts = new List<PointF>();

            this.Cursor = Cursors.WaitCursor;
            this.Refresh();

            bool pc = this.CheckBox16.Checked;
            if (pc)
                Button22.PerformClick();

            if (CurPath != null && CurPath.Count > 0)
            {
                for (int i = 0; i <= CurPath.Count - 1; i++)
                {
                    List<PointF> p = CurPath[i];

                    if (_cloneCurPath)
                    {
                        PointF[] l = new PointF[p.Count - 1 + 1];
                        p.CopyTo(l);
                        pathCopy.Add(l.ToList());
                    }

                    if (p != null && p.Count > 1)
                    {
                        for (int j = 0; j <= p.Count - 1; j++)
                            pts.Add(new PointF(p[j].X, p[j].Y));
                    }
                }
            }

            float w = System.Convert.ToSingle(this.NumericUpDown21.Value);

            if (this.ComboBox8.SelectedIndex == 0)
            {
                float c = System.Convert.ToSingle(this.NumericUpDown28.Value);
                int mode = this.ComboBox7.SelectedIndex;

                List<PointF> pts2 = new List<PointF>();
                PointF[] pts3 = pts.ToArray();

                if (CheckUmlauf(pts3) == false)
                    pts3 = pts3.Reverse().ToArray();

                List<PointF> pts3a = new List<PointF>();
                pts3a.AddRange(pts3);

                for (int j = 1; j <= pts3a.Count - 1; j++)
                {
                    // //while((j < temp.length) && (Math.sqrt( Math.pow(Math.abs((temp[j - 1] % w) - (temp[j] % w)), 2) ) + 
                    // //	( Math.pow(Math.abs(Math.floor(temp[j - 1] / w) - Math.floor(temp[j] / w)), 2) ) < 4))
                    if (mode == 0)
                    {
                        while ((j < pts3a.Count) && ((Math.Abs(pts3a[j - 1].X - pts3a[j].X) < c) || (Math.Abs(pts3a[j - 1].Y - pts3a[j].Y) < c)))
                            pts3a.RemoveAt(j);
                    }
                    else if (mode == 1)
                    {
                        while ((j < pts3a.Count) && (Math.Sqrt((pts3a[j - 1].X - pts3a[j].X) * (pts3a[j - 1].X - pts3a[j].X) + (pts3a[j - 1].Y - pts3a[j].Y) * (pts3a[j - 1].Y - pts3a[j].Y)) < Math.Pow(c, 2)))
                            pts3a.RemoveAt(j);
                    }
                    else
                        while ((j < pts3a.Count) && ((Math.Abs(pts3a[j - 1].X - pts3a[j].X) < c) || (Math.Abs(pts3a[j - 1].Y - pts3a[j].Y) < c)) && (Math.Sqrt((pts3a[j - 1].X - pts3a[j].X) * (pts3a[j - 1].X - pts3a[j].X) + (pts3a[j - 1].Y - pts3a[j].Y) * (pts3a[j - 1].Y - pts3a[j].Y)) < Math.Pow(c, 2)))
                            pts3a.RemoveAt(j);
                }

                pts3 = pts3a.ToArray();

                for (int i = 0; i <= pts3.Length - 1; i++)
                {
                    PointF pt = GetWidenedPoint(pts3, i, w);
                    if (!float.IsNaN(pt.X) && !float.IsNaN(pt.Y))
                        pts2.Add(pt);
                }

                // //close
                if (pts2.Count > 1)
                {
                    if (pts2[0].X != pts2[pts2.Count - 1].X || pts2[0].Y != pts2[pts2.Count - 1].Y)
                        pts2.Add(pts2[0]);

                    this.CurPath = new List<List<PointF>>();
                    this.CurPath.Add(pts2);

                    if (_cloneCurPath)
                    {
                        _curPathCopy = pathCopy;
                        _cloneCurPath = false;
                        this.Button9.Enabled = true;
                    }

                    TranslatePathToZoom(this.CurPath);
                    PrepareWorker();
                    this.BackgroundWorker1.RunWorkerAsync();
                }
                else
                    MessageBox.Show("path is empty");
            }
            else if (this.ComboBox8.SelectedIndex == 1)
            {
                List<PointF> pts2 = new List<PointF>();

                if (pts.Count > 2)
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddLines(pts.ToArray());

                        using (GraphicsPath gp2 = ShiftCoords(gp, -w * this.HelplineRulerCtrl1.Zoom, false, false))
                        {
                            if (gp2.PointCount > 0)
                            {
                                PointF[] ptsTemp = new PointF[gp2.PathPoints.Length];
                                gp2.PathPoints.CopyTo(ptsTemp, 0);
                                pts2.AddRange(ptsTemp);
                            }
                        }
                    }

                    this.CurPath = new List<List<PointF>>();
                    this.CurPath.Add(pts2);

                    if (_cloneCurPath)
                    {
                        _curPathCopy = pathCopy;
                        _cloneCurPath = false;
                        this.Button9.Enabled = true;
                    }

                    TranslatePathToZoom(this.CurPath);
                    PrepareWorker();
                    this.BackgroundWorker1.RunWorkerAsync();
                }
            }
            else
            {
                List<PointF> pts2 = new List<PointF>();

                if (pts.Count > 2)
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        gp.AddLines(pts.ToArray());

                        Bitmap? bmp = null;

                        if (AvailMem.AvailMem.checkAvailRam(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Bmp.Height * 4L))
                        {
                            bmp = new Bitmap(this.HelplineRulerCtrl1.Bmp.Width, this.HelplineRulerCtrl1.Bmp.Height);
                            using (Graphics gx = Graphics.FromImage(bmp))
                            {
                                gx.SmoothingMode = SmoothingMode.AntiAlias;

                                using (TextureBrush tb = new TextureBrush(this.HelplineRulerCtrl1.Bmp))
                                {
                                    gx.FillPath(tb, gp);
                                }
                            }
                            if (this.CurPath != null)
                                using (frmOutlineShift frm = new frmOutlineShift(bmp, this.CurPath))
                                {
                                    if (frm.ShowDialog() == DialogResult.OK && frm.ALF2 != null)
                                    {
                                        using (GraphicsPath gp2 = frm.ALF2)
                                        {
                                            if (gp2 != null && gp2.PointCount > 0)
                                            {
                                                PointF[] ptsTemp = new PointF[gp2.PathPoints.Length - 1 + 1];
                                                gp2.PathPoints.CopyTo(ptsTemp, 0);
                                                int i = -1;
                                                try
                                                {
                                                    // i = gp2.PathTypes.ToList().IndexOf(gp2.PathTypes.First(Function(a) a = 129 OrElse (a = 0 AndAlso gp2.PathTypes.ToList().IndexOf(a) > 0)))
                                                    i = gp2.PathTypes.ToList().IndexOf(gp2.PathTypes.First(a => a == 0 && gp2.PathTypes.ToList().IndexOf(a) > 0));
                                                }
                                                catch
                                                {
                                                    i = gp2.PointCount - 1;
                                                }

                                                if (i > -1)
                                                {
                                                    int figure1End = i;
                                                    pts2.AddRange(ptsTemp.Take(figure1End));
                                                }
                                            }
                                        }
                                    }
                                }
                        }
                    }

                    if (pts2.Count > 2)
                    {
                        this.CurPath = new List<List<PointF>>();
                        this.CurPath.Add(pts2);

                        if (_cloneCurPath)
                        {
                            _curPathCopy = pathCopy;
                            _cloneCurPath = false;
                            this.Button9.Enabled = true;
                        }

                        TranslatePathToZoom(this.CurPath);
                        PrepareWorker();
                        this.BackgroundWorker1.RunWorkerAsync();
                    }
                }
            }
            SetUpListBox();
            this.Button9.Enabled = true;
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            this.Cursor = Cursors.Default;
        }

        private bool CheckUmlauf(PointF[] list)
        {
            double cAngle = 0;
            double cAngle2 = 0;

            if (list.Length > 1)
            {
                int h = list.Length / 2;

                for (int i = 0; i <= list.Length - 1; i++)
                {
                    PointF p1 = new PointF(list[i].X, list[i].Y);
                    int j = (i + h) % list.Length;
                    PointF p2 = new PointF(list[j].X, list[j].Y);
                    double dx = p2.X - p1.X;
                    double dy = p2.Y - p1.Y;
                    double w = Math.Atan2(dy, dx);
                    double dw = w - cAngle;
                    cAngle += dw;
                    if (i == 0)
                        cAngle2 = cAngle;
                }
            }

            if ((cAngle2 - cAngle) < 0)
                return false;

            return true; // //clockwise;
        }

        private PointF GetWidenedPoint(PointF[] points, int indx, float widen)
        {
            if (points.Length > 1)
            {
                if (points.Length == 2)
                    return new PointF(points[indx].X, points[indx].Y);
                else
                {
                    int i = indx;
                    int j = indx - 1;
                    if (i == 0)
                        j = points.Length - 1;
                    int l = indx + 1;
                    if (i == points.Length - 1)
                        l = 0;

                    PointF pt1 = new PointF(points[i].X, points[i].Y);
                    PointF pt2 = new PointF(points[j].X, points[j].Y);
                    PointF pt3 = new PointF(points[l].X, points[l].Y);

                    PointF ds = new PointF(pt3.X - pt2.X, pt3.Y - pt2.Y);
                    double ls = Math.Sqrt(ds.X * ds.X + ds.Y * ds.Y);
                    PointF n1 = new PointF(System.Convert.ToSingle(ds.Y / ls), System.Convert.ToSingle(-ds.X / ls));
                    PointF n2 = new PointF(n1.X * widen, n1.Y * widen);

                    PointF t = new PointF(pt1.X + n2.X, pt1.Y + n2.Y);

                    return new PointF(t.X, t.Y);
                }
            }

            return new PointF(-1, -1);
        }

        private PointF GetWidenedPointRev(PointF[] points, int indx, float widen)
        {
            if (points.Length > 1)
            {
                if (points.Length == 2)
                    return new PointF(points[indx].X, points[indx].Y);
                else
                {
                    int i = indx;
                    int j = indx - 1;
                    if (i == 0)
                        j = points.Length - 1;
                    int l = indx + 1;
                    if (i == points.Length - 1)
                        l = 0;

                    PointF pt1 = new PointF(points[i].X, points[i].Y);
                    PointF pt2 = new PointF(points[j].X, points[j].Y);
                    PointF pt3 = new PointF(points[l].X, points[l].Y);

                    PointF ds = new PointF(pt3.X - pt2.X, pt3.Y - pt2.Y);
                    double ls = Math.Sqrt(ds.X * ds.X + ds.Y * ds.Y);
                    PointF n1 = new PointF(System.Convert.ToSingle(-ds.Y / ls), System.Convert.ToSingle(ds.X / ls));
                    PointF n2 = new PointF(n1.X * widen, n1.Y * widen);

                    PointF t = new PointF(pt1.X + n2.X, pt1.Y + n2.Y);

                    return new PointF(t.X, t.Y);
                }
            }

            return new PointF(-1, -1);
        }

        private void ComboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ComboBox8.SelectedIndex == 0)
            {
                this.ComboBox7.Enabled = true;
                this.NumericUpDown21.Enabled = true;
                this.NumericUpDown28.Enabled = true;
                this.CheckBox16.Enabled = true;
                this.Label38.Enabled = true;
                this.Label39.Enabled = true;
            }
            else
            {
                this.ComboBox7.Enabled = false;
                this.NumericUpDown21.Enabled = true;
                this.NumericUpDown28.Enabled = false;
                this.CheckBox16.Enabled = false;
                this.Label38.Enabled = false;
                this.Label39.Enabled = false;
            }
        }

        private void CheckBox18_CheckedChanged(object sender, EventArgs e)
        {
            this._drawSinglePoint = !this.CheckBox18.Checked;
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }


        public GraphicsPath ShiftCoords(GraphicsPath gP, float shiftInwards, bool mapLargestOnly, bool stretch)
        {
            if (gP.PointCount > 1)
            {
                // do it separately for each PathFigure...
                byte[] t = new byte[gP.PathTypes.Length - 1 + 1];
                gP.PathTypes.CopyTo(t, 0);
                PointF[] pts = new PointF[gP.PathPoints.Length - 1 + 1];
                gP.PathPoints.CopyTo(pts, 0);

                List<List<byte>> pTAll = new List<List<byte>>();
                List<List<PointF>> pPAll = new List<List<PointF>>();

                int cnt = 0;

                while (cnt < t.Length)
                {
                    List<byte> pT = new List<byte>();
                    List<PointF> pP = new List<PointF>();

                    pP.Add(pts[cnt]);
                    pT.Add(0); // pT.Add(t(cnt))
                    cnt += 1;

                    while (cnt < t.Length && t[cnt] != 0)
                    {
                        pP.Add(pts[cnt]);
                        pT.Add(t[cnt]);
                        cnt += 1;
                    }

                    pPAll.Add(pP);
                    pT[pT.Count - 1] = 129; // fffffff
                    pTAll.Add(pT);
                }

                pPAll = pPAll.OrderByDescending(a => a.Count).ToList();
                pTAll = pTAll.OrderByDescending(a => a.Count).ToList();

                GraphicsPath shiftedPath = new GraphicsPath();

                for (int cnt2 = 0; cnt2 <= pPAll.Count - 1; cnt2++)
                {
                    if ((mapLargestOnly && cnt2 == 0) || !mapLargestOnly)
                    {
                        if (pPAll[cnt2].Count > 1)
                        {
                            using (GraphicsPath outerPath = new GraphicsPath(pPAll[cnt2].ToArray(), pTAll[cnt2].ToArray()))
                            {
                                if (outerPath.PointCount > 1)
                                {
                                    PointF[] outerPts = new PointF[outerPath.PathPoints.Length - 1 + 1];
                                    outerPath.PathPoints.CopyTo(outerPts, 0);
                                    RectangleF rc = outerPath.GetBounds();
                                    int w = System.Convert.ToInt32(Math.Ceiling(rc.Width));
                                    int h = System.Convert.ToInt32(Math.Ceiling(rc.Height));

                                    if (w > 0 && h > 0)
                                    {
                                        Bitmap? bmpTmp = null;
                                        Bitmap? bmpInner = null;

                                        int breite = System.Convert.ToInt32(Math.Ceiling(shiftInwards));
                                        double fraction = shiftInwards / (double)breite != 0 ? breite : shiftInwards - (System.Convert.ToInt32(shiftInwards));

                                        if (shiftInwards < 0)
                                        {
                                            Bitmap? bmpTmp2 = null;
                                            Bitmap? bmpOuter = null;

                                            try
                                            {
                                                int br = System.Convert.ToInt32(Math.Floor(shiftInwards));
                                                if (AvailMem.AvailMem.checkAvailRam((w - br * 2) * (h - br * 2) * 4L))
                                                {
                                                    bmpTmp2 = new Bitmap(w - br * 2, h - br * 2);
                                                    using (GraphicsPath outerPath2 = (GraphicsPath)outerPath.Clone())
                                                    {
                                                        using (Pen p = new Pen(Color.Black, -br))
                                                        {
                                                            using (Matrix mx = new Matrix(1, 0, 0, 1, -rc.X - br, -rc.Y - br))
                                                            {
                                                                outerPath2.Transform(mx);
                                                            }

                                                            using (Graphics g = Graphics.FromImage(bmpTmp2))
                                                            {
                                                                g.FillPath(Brushes.Black, outerPath2);
                                                            }

                                                            bmpOuter = ExtOutline(bmpTmp2, -breite, null);

                                                            if (bmpOuter != null)
                                                            {
                                                                List<ChainCodeF> lOuter = GetBoundaryF(bmpOuter);

                                                                outerPath.Reset();
                                                                for (int i = 0; i <= lOuter.Count - 1; i++)
                                                                {
                                                                    // translate
                                                                    for (int j = 0; j <= lOuter[i].Coord.Count - 1; j++)
                                                                        lOuter[i].Coord[j] = new PointF(lOuter[i].Coord[j].X + rc.X + br, lOuter[i].Coord[j].Y + rc.Y + br);
                                                                    outerPath.AddLines(lOuter[i].Coord.ToArray());
                                                                    outerPath.CloseFigure();
                                                                }

                                                                PointF[] outerPts2 = new PointF[outerPath.PathPoints.Length - 1 + 1];
                                                                outerPath.PathPoints.CopyTo(outerPts2, 0);
                                                                outerPts = outerPts2;
                                                                rc = outerPath.GetBounds();
                                                                w = System.Convert.ToInt32(Math.Ceiling(rc.Width));
                                                                h = System.Convert.ToInt32(Math.Ceiling(rc.Height));
                                                                shiftInwards *= -1;
                                                                fraction = 1.0 - fraction;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            }

                                            finally
                                            {
                                                if (bmpTmp2 != null)
                                                {
                                                    bmpTmp2.Dispose();
                                                    bmpTmp2 = null;
                                                }
                                                if (bmpOuter != null)
                                                {
                                                    bmpOuter.Dispose();
                                                    bmpOuter = null;
                                                }
                                            }
                                        }

                                        if (outerPath.PointCount > 4)
                                        {
                                            List<PointF> shiftedPathPts = new List<PointF>();
                                            List<byte> shiftedPathTypes = new List<byte>();

                                            if (AvailMem.AvailMem.checkAvailRam(w * h * 8L))
                                            {
                                                try
                                                {
                                                    bmpTmp = new Bitmap(w, h);
                                                    using (Graphics g = Graphics.FromImage(bmpTmp))
                                                    {
                                                        using (Matrix mx = new Matrix(1, 0, 0, 1, -rc.X, -rc.Y))
                                                        {
                                                            outerPath.Transform(mx);
                                                        }
                                                        g.FillPath(Brushes.Black, outerPath);
                                                        using (Matrix mx = new Matrix(1, 0, 0, 1, rc.X, rc.Y))
                                                        {
                                                            outerPath.Transform(mx);
                                                        }

                                                        bmpInner = RemOutline(bmpTmp, breite, null);

                                                        if (bmpInner != null)
                                                        {
                                                            // get the new outline (path) and prolongate it to have the same amount of points as the large path
                                                            // then find the closest points and resample
                                                            // innerPath can have more figures than outerPath
                                                            List<ChainCode> lInner = GetBoundary(bmpInner);

                                                            if (lInner.Count > 0)
                                                            {
                                                                lInner = lInner.OrderByDescending(a => a.Coord.Count).ToList();

                                                                List<List<PointF>> newInnerPtsAll = new List<List<PointF>>();

                                                                if (mapLargestOnly)
                                                                {
                                                                    List<PointF> newInnerPts = new List<PointF>();
                                                                    for (int j = 0; j <= lInner[0].Coord.Count - 1; j++)
                                                                        newInnerPts.Add(new PointF(lInner[0].Coord[j].X, lInner[0].Coord[j].Y));
                                                                    newInnerPtsAll.Add(newInnerPts);
                                                                }
                                                                else
                                                                    for (int i = 0; i <= lInner.Count - 1; i++)
                                                                    {
                                                                        List<PointF> newInnerPts = new List<PointF>();
                                                                        for (int j = 0; j <= lInner[i].Coord.Count - 1; j++)
                                                                            newInnerPts.Add(new PointF(lInner[i].Coord[j].X, lInner[i].Coord[j].Y));
                                                                        newInnerPtsAll.Add(newInnerPts);
                                                                    }

                                                                double factor = outerPath.PointCount / (double)lInner.Sum(a => a.Coord.Count);
                                                                double dFracSum = 0.0;
                                                                int pointCountAll = 0;

                                                                for (int l = 0; l <= newInnerPtsAll.Count - 1; l++)
                                                                {
                                                                    double dLength = newInnerPtsAll[l].Count * factor;
                                                                    int pointCount = System.Convert.ToInt32(Math.Ceiling(dLength));
                                                                    dFracSum += pointCount - dLength;

                                                                    if (dFracSum > 1.0)
                                                                    {
                                                                        dFracSum -= 1;
                                                                        pointCount -= 1;
                                                                    }

                                                                    pointCountAll += pointCount;

                                                                    if (pointCountAll > outerPath.PointCount)
                                                                        pointCount -= pointCountAll - outerPath.PointCount;

                                                                    newInnerPtsAll[l] = TranslateNewInnerPath(newInnerPtsAll[l], rc);
                                                                    List<PointF> newInnerPathPoints = newInnerPtsAll[l];
                                                                    if (stretch)
                                                                        ProlongateInnerPath(newInnerPtsAll[l], pointCount, factor);

                                                                    bool bIsClosed = false;

                                                                    if (newInnerPtsAll[l][newInnerPtsAll[l].Count - 1].X == newInnerPtsAll[l][0].X && newInnerPtsAll[l][newInnerPtsAll[l].Count - 1].Y == newInnerPtsAll[l][0].Y)
                                                                    {
                                                                        newInnerPathPoints[newInnerPathPoints.Count - 1] = new PointF(newInnerPathPoints[0].X, newInnerPathPoints[0].Y);
                                                                        bIsClosed = true;
                                                                    }

                                                                    double outerFraction = 1.0 - fraction;

                                                                    pTAll[cnt2][shiftedPathPts.Count - System.Convert.ToInt32(Math.Floor(dFracSum))] = 0;

                                                                    // since the shift back is less or equal 1, loop over the innerPoints and set FillMode to Winding
                                                                    // for keeping the amount of figures of the innerPath
                                                                    for (int i = 0; i <= newInnerPathPoints.Count - 1; i++)
                                                                    {
                                                                        double minDist = double.MaxValue;
                                                                        int minIndex = -1;

                                                                        for (int j = 0; j <= outerPts.Count() - 1; j++)
                                                                        {
                                                                            double dx = outerPts[j].X - newInnerPathPoints[i].X;
                                                                            double dy = outerPts[j].Y - newInnerPathPoints[i].Y;
                                                                            double dist = Math.Sqrt(dx * dx + dy * dy);

                                                                            if (dist < minDist)
                                                                            {
                                                                                minDist = dist;
                                                                                minIndex = j;
                                                                            }
                                                                        }

                                                                        if (minIndex > -1)
                                                                        {
                                                                            float outerX = outerPts[minIndex].X;
                                                                            float outerY = outerPts[minIndex].Y;
                                                                            float innerX = newInnerPathPoints[i].X;
                                                                            float innerY = newInnerPathPoints[i].Y;

                                                                            double newX = innerX * fraction + outerX * outerFraction;
                                                                            double newY = innerY * fraction + outerY * outerFraction;

                                                                            shiftedPathPts.Add(new PointF(System.Convert.ToSingle(newX), System.Convert.ToSingle(newY)));
                                                                            if (i > 0)
                                                                                shiftedPathTypes.Add(1);
                                                                            else
                                                                                shiftedPathTypes.Add(0);
                                                                        }
                                                                    }

                                                                    if (bIsClosed)
                                                                    {
                                                                        pTAll[cnt2][shiftedPathPts.Count - 1] = 129;
                                                                        shiftedPathTypes[shiftedPathTypes.Count - 1] = 129;
                                                                    }
                                                                }

                                                                if (stretch && shiftedPathPts.Count != outerPath.PointCount)
                                                                    shiftedPathPts = RestrictOuterPath(shiftedPathPts, outerPath.PointCount);
                                                            }
                                                            else
                                                            {
                                                            }
                                                        }
                                                    }
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
                                                    if (bmpInner != null)
                                                    {
                                                        bmpInner.Dispose();
                                                        bmpInner = null;
                                                    }
                                                }
                                            }

                                            if (shiftedPathPts.Count > 1)
                                            {
                                                if (stretch)
                                                {
                                                    if (shiftedPathPts.Count > 1)
                                                    {
                                                        GraphicsPath gP4 = new GraphicsPath(shiftedPathPts.ToArray(), pTAll[cnt2].ToArray());
                                                        gP4.FillMode = FillMode.Winding;
                                                        shiftedPath.AddPath(gP4, false);
                                                    }
                                                }
                                                else if (shiftedPathPts.Count > 1)
                                                {
                                                    GraphicsPath gP4 = new GraphicsPath(shiftedPathPts.ToArray(), shiftedPathTypes.ToArray());
                                                    gP4.FillMode = FillMode.Winding;
                                                    shiftedPath.AddPath(gP4, false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                        break;
                }

                return shiftedPath;
            }
            else
                return new GraphicsPath();
        }

        private List<PointF> TranslateNewInnerPath(List<PointF> newInnerPath, RectangleF rc)
        {
            List<PointF> lOut = new List<PointF>();

            for (int i = 0; i <= newInnerPath.Count - 1; i++)
                lOut.Add(new PointF(newInnerPath[i].X + rc.X, newInnerPath[i].Y + rc.Y));

            return lOut;
        }

        private List<PointF> ProlongateInnerPath(List<PointF> lInner, int pointCount, double factor)
        {
            PointF[] pts = new PointF[pointCount - 1 + 1];

            for (int i = 0; i <= pts.Length - 1; i++)
            {
                double d = i / factor;

                int l = System.Convert.ToInt32(Math.Floor(d));
                int r = System.Convert.ToInt32(Math.Ceiling(d));
                double f = d - l;
                double t = 1 - f;

                if (l > lInner.Count - 1)
                    l -= 1;
                if (r > lInner.Count - 1)
                    r -= 1;

                double x = lInner[l].X * f + lInner[r].X * t;
                double y = lInner[l].Y * f + lInner[r].Y * t;

                pts[i] = new PointF(System.Convert.ToSingle(x), System.Convert.ToSingle(y));
            }

            return pts.ToList();
        }

        private List<PointF> RestrictOuterPath(List<PointF> lInner, int pointCount)
        {
            double factor = pointCount / (double)lInner.Count;
            PointF[] pts = new PointF[pointCount - 1 + 1];

            for (int i = 0; i <= pts.Length - 1; i++)
            {
                double d = i / factor;

                int l = System.Convert.ToInt32(Math.Floor(d));
                int r = System.Convert.ToInt32(Math.Ceiling(d));
                double f = d - l;
                double t = 1 - f;

                if (l > lInner.Count - 1)
                    l -= 1;
                if (r > lInner.Count - 1)
                    r -= 1;

                double x = lInner[l].X * f + lInner[r].X * t;
                double y = lInner[l].Y * f + lInner[r].Y * t;

                pts[i] = new PointF(System.Convert.ToSingle(x), System.Convert.ToSingle(y));
            }

            return pts.ToList();
        }

        private List<ChainCode> GetBoundary(Bitmap upperImg)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(upperImg, 0, false, 0, false, 0, false);
            return l;
        }

        private List<ChainCodeF> GetBoundaryF(Bitmap upperImg)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCodeF> l = cf.GetOutline(0, upperImg, 0, false, false);
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

                    for (int i = 0; i <= breite - 1; i++)
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

        public Bitmap? ExtOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
            {
                Bitmap? b = null;
                BitArray? fbits = null;

                try
                {
                    b = (Bitmap)bmp.Clone();

                    for (int i = 0; i <= breite - 1; i++)
                    {
                        if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                            break;

                        ChainFinder cf = new ChainFinder();
                        cf.AllowNullCells = true;

                        List<ChainCode> fList = cf.GetOutline(b, 0, false, 0, false);

                        cf.ExtendOutline(b, fList);

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


        private void Button23_Click(object sender, EventArgs e)
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();
            List<PointF> pts = new List<PointF>();

            float up = System.Convert.ToSingle(this.NumericUpDown22.Value);
            float down = System.Convert.ToSingle(this.NumericUpDown23.Value);
            float Left = System.Convert.ToSingle(this.NumericUpDown24.Value);
            float right = System.Convert.ToSingle(this.NumericUpDown25.Value);

            if (CurPath != null && CurPath.Count > 0)
            {
                for (int i = 0; i <= CurPath.Count - 1; i++)
                {
                    List<PointF> p = CurPath[i];

                    if (_cloneCurPath)
                    {
                        PointF[] l = new PointF[p.Count - 1 + 1];
                        p.CopyTo(l);
                        pathCopy.Add(l.ToList());
                    }

                    if (p != null && p.Count > 1)
                    {
                        for (int j = 0; j <= p.Count - 1; j++)
                            pts.Add(new PointF(p[j].X + right - Left, p[j].Y - up + down));
                    }
                }

                // //close
                if (pts.Count > 1)
                {
                    if (pts[0].X != pts[pts.Count - 1].X || pts[0].Y != pts[pts.Count - 1].Y)
                        pts.Add(pts[0]);

                    this.CurPath = new List<List<PointF>>();
                    this.CurPath.Add(pts);

                    if (_cloneCurPath)
                    {
                        _curPathCopy = pathCopy;
                        _cloneCurPath = false;
                        this.Button9.Enabled = true;
                    }

                    TranslatePathToZoom(this.CurPath);
                    PrepareWorker();
                    this.BackgroundWorker1.RunWorkerAsync();
                }
                else
                    MessageBox.Show("path is empty");
            }
            this.Button9.Enabled = true;
            SetUpListBox();
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button22_Click(object sender, EventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null)
            {
                Bitmap? b = null;
                if (AvailMem.AvailMem.checkAvailRam(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Bmp.Height * 4L))
                    b = new Bitmap(this.HelplineRulerCtrl1.Bmp.Width, this.HelplineRulerCtrl1.Bmp.Height);
                else
                {
                    MessageBox.Show("Not enough Memory");
                    return;
                }

                List<List<PointF>> pathCopy = new List<List<PointF>>();

                using (Graphics gx = Graphics.FromImage(b))
                {
                    // gx.SmoothingMode = SmoothingMode.AntiAlias
                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    gx.PixelOffsetMode = PixelOffsetMode.Half;
                    gx.CompositingQuality = CompositingQuality.HighQuality;

                    // gx.TranslateTransform(0.5F, 0.5F)

                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (CurPath != null && CurPath.Count > 0)
                        {
                            for (int i = CurPath.Count - 1; i >= 0; i += -1)
                            {
                                List<PointF> p = CurPath[i];
                                if (p.Count == 0)
                                    CurPath.RemoveAt(i);
                            }
                            for (int i = 0; i <= CurPath.Count - 2; i++)
                            {
                                List<PointF> p = CurPath[i];
                                List<PointF> q = CurPath[i + 1];
                                if (p[p.Count - 1].X == q[0].X && p[p.Count - 1].Y == q[0].Y)
                                    q.RemoveAt(0);
                            }
                            for (int i = 0; i <= CurPath.Count - 1; i++)
                            {
                                List<PointF> p = CurPath[i];
                                if (p.Count > 1)
                                {
                                    for (int j = p.Count - 2; j >= 0; j += -1)
                                    {
                                        if (p[j].X == p[j + 1].X && p[j].Y == p[j + 1].Y)
                                            p.RemoveAt(j + 1);
                                    }
                                }
                                if (p != null && p.Count > 1)
                                {
                                    if (_cloneCurPath)
                                    {
                                        PointF[] l = new PointF[p.Count - 1 + 1];
                                        p.CopyTo(l);
                                        pathCopy.Add(l.ToList());
                                    }

                                    gp.AddLines(p.ToArray());
                                }
                            }
                            gp.CloseAllFigures();
                        }
                        float w = Math.Max(System.Convert.ToSingle(this.NumericUpDown3.Value), 8);

                        using (Pen pen = new Pen(Color.Black, w))
                        {
                            pen.LineJoin = LineJoin.Round;
                            pen.Alignment = PenAlignment.Inset;
                            // gp.Widen(pen)
                            gx.SetClip(gp);
                            gx.DrawPath(pen, gp);
                        }
                    }
                }

                ChainFinder cf = new ChainFinder();
                cf.AllowNullCells = false;

                List<ChainCode> fList = cf.GetOutline(b, 0, false, 0, true);
                this.CurPath = new List<List<PointF>>();

                for (int j = 0; j < fList.Count; j++)
                {
                    List<PointF> c = fList[j].Coord.ConvertAll(a => new PointF(a.X, a.Y));
                    c = cf.RemoveColinearity(c, true);

                    PointF[] l = new PointF[c.Count];
                    c.CopyTo(l);

                    this.CurPath.Add(l.ToList());
                }

                if (_cloneCurPath)
                {
                    _curPathCopy = pathCopy;
                    _cloneCurPath = false;
                    this.Button9.Enabled = true;
                }

                b.Dispose();
                b = null;

                TranslatePathToZoom(this.CurPath);
                PrepareWorker();
                this.BackgroundWorker1.RunWorkerAsync();
                // GetWidenedPoints()
                this.Button9.Enabled = true;
                SetUpListBox();
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.HelplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.HelplineRulerCtrl1.Enabled = false;
                this.HelplineRulerCtrl1.Refresh();
                this.HelplineRulerCtrl1.SetZoom(ComboBox2.SelectedItem?.ToString());
                this.HelplineRulerCtrl1.Enabled = true;
                if (this.ComboBox2.SelectedIndex < 2)
                    this.HelplineRulerCtrl1.ZoomSetManually = true;

                TranslatePathToZoom(this.CurPath);
                PrepareWorker();
                this.BackgroundWorker1.RunWorkerAsync();
                // GetWidenedPoints()
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                this._curZoom = this.HelplineRulerCtrl1.Zoom;
            }
        }

        private void frmEditPath_Load(object sender, EventArgs e)
        {
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                this.Enabled = false;
                this.Timer1.Start();
            }
            else if (this.HelplineRulerCtrl1.Bmp != null)
            {
                if (this.CurPath == null)
                    this.CurPath = new List<List<PointF>>();
                List<PointF> l = new List<PointF>();
                l.Add(new PointF(0, 0));
                l.Add(new PointF(this.HelplineRulerCtrl1.Bmp.Width, 0));
                l.Add(new PointF(this.HelplineRulerCtrl1.Bmp.Width, this.HelplineRulerCtrl1.Bmp.Height));
                l.Add(new PointF(0, this.HelplineRulerCtrl1.Bmp.Height));
                l.Add(new PointF(0, 0));
                this.CurPath.Add(l);
                this.Enabled = false;
                this.Timer1.Start();
            }

            this.ComboBox2.Items.Add((0.75F).ToString());
            this.ComboBox2.Items.Add((0.5F).ToString());
            this.ComboBox2.Items.Add((0.25F).ToString());
        }

        private void SetUpListBox()
        {
            this.ListBox1.Items.Clear();
            this.ListBox1.SuspendLayout();
            this.ListBox1.BeginUpdate();

            if (CurPath != null)
            {
                if (CurPath.Count > 0)
                {
                    int cnt = 0;
                    List<PointF> l = new List<PointF>();
                    for (int i = 0; i < CurPath.Count; i++)
                    {
                        List<PointF> p = CurPath[i];

                        if (p.Count > 0)
                        {
                            for (int j = 0; j < p.Count; j++)
                                this.ListBox1.Items.Add("X: " + p[j].X + "; Y: " + p[j].Y);
                            cnt = p.Count;
                            l.AddRange(p.ToArray());
                        }
                    }

                    this._pointCount = cnt;
                    this._listCorrespondingPts = l;

                    if (this.ListBox1.Items.Count > 0)
                        this.ListBox1.SelectedIndex = 0;
                }
            }
            this.ListBox1.EndUpdate();
            this.ListBox1.ResumeLayout();
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.ListBox1.IsDisposed && this.ListBox1.SelectedIndex > -1 && this._listCorrespondingPts != null)
            {
                this.Label2.Text = "pt " + this.ListBox1.SelectedIndex + " of " + this._pointCount.ToString();

                this.NumericUpDown1.Value = System.Convert.ToDecimal(this._listCorrespondingPts[this.ListBox1.SelectedIndex].X);
                this.NumericUpDown2.Value = System.Convert.ToDecimal(this._listCorrespondingPts[this.ListBox1.SelectedIndex].Y);

                this._selctedIndex = this.ListBox1.SelectedIndex;
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            if (this.ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                this._fColor = this.ColorDialog1.Color;
                this.Button13.BackColor = this._fColor;
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null)
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null && this.CurPath != null && this.CurPath.Count == 1)
            {
                if (!this.ListBox1.IsDisposed && this.ListBox1.Items.Count > 0 && this.ListBox1.SelectedIndex > -1 && this._listCorrespondingPts != null)
                {
                    PointF p = new PointF(System.Convert.ToSingle(this.NumericUpDown1.Value), System.Convert.ToSingle(this.NumericUpDown2.Value));
                    float offsetX = System.Convert.ToSingle(this.NumericUpDown1.Value) - this.CurPath[0][this.ListBox1.SelectedIndex].X;
                    float offsetY = System.Convert.ToSingle(this.NumericUpDown2.Value) - this.CurPath[0][this.ListBox1.SelectedIndex].Y;
                    this.ListBox1.Items[this.ListBox1.SelectedIndex] = "X: " + p.X + "; Y: " + p.Y;
                    this.CurPath[0][this.ListBox1.SelectedIndex] = p;
                    this._listCorrespondingPts[this.ListBox1.SelectedIndex] = p;
                    CPZTranslatePathPointToZoom(this.ListBox1.SelectedIndex, p);
                    GetWidenedPoint(this.ListBox1.SelectedIndex, new PointF(offsetX, offsetY));
                    this.Button9.Enabled = true;
                    this.NumericUpDown1.Value = System.Convert.ToDecimal(this._listCorrespondingPts[this.ListBox1.SelectedIndex].X);
                    this.NumericUpDown2.Value = System.Convert.ToDecimal(this._listCorrespondingPts[this.ListBox1.SelectedIndex].Y);
                }
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void GetWidenedPoint(int selectedIndex, PointF offset)
        {
            if (this.HelplineRulerCtrl1.Bmp != null && this.CurPath != null && this.CurPath.Count == 1)
            {
                if (this._widenedPoints != null)
                    this._widenedPoints[selectedIndex] = new PointF(this._widenedPoints[selectedIndex].X + offset.X, this._widenedPoints[selectedIndex].Y + offset.Y);
                if (this._widenedPointsZ != null)
                    this._widenedPointsZ[selectedIndex] = new PointF(this._widenedPointsZ[selectedIndex].X + offset.X * this.HelplineRulerCtrl1.Zoom, this._widenedPointsZ[selectedIndex].Y + offset.Y * this.HelplineRulerCtrl1.Zoom);
            }
        }

        private void CPZTranslatePathPointToZoom(int selectedIndex, PointF p)
        {
            if (this.HelplineRulerCtrl1.Bmp != null && this.CurPath != null && this.CurPath.Count == 1)
            {
                if (this.CurPathZ != null && this.CurPathZ.Count == 1 && this.CurPathZ[0].Count > selectedIndex)
                    this.CurPathZ[0][selectedIndex] = new PointF(p.X * this.HelplineRulerCtrl1.Zoom, p.Y * this.HelplineRulerCtrl1.Zoom);
                else if (this.CurPathZ != null && this.CurPathZ.Count == 1)
                    this.CurPathZ[0].Add(new PointF(p.X * this.HelplineRulerCtrl1.Zoom, p.Y * this.HelplineRulerCtrl1.Zoom));
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null && this.CurPath != null && this.CurPath.Count == 1)
            {
                PrepareWorker();
                this.BackgroundWorker1.RunWorkerAsync();
                // GetWidenedPoints()
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void GetWidenedPoints(System.ComponentModel.BackgroundWorker bgw)
        {
            try
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (CurPath != null && CurPath.Count > 0)
                    {
                        for (int i = 0; i <= CurPath.Count - 1; i++)
                        {
                            if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                                return;
                            List<PointF> p = CurPath[i];
                            if (p != null && p.Count > 1)
                                gp.AddLines(p.ToArray());
                        }
                    }
                    if (gp.PointCount > 2)
                    {
                        PointF[] p = new PointF[gp.PathPoints.Length];
                        gp.PathPoints.CopyTo(p, 0);

                        double f = 100.0 / p.Length;
                        bool rev = false;
                        if ((!CheckUmlauf(p)))
                            rev = true;
                        if (this.CheckBox2.Checked)
                            rev = !rev;
                        this._widenedPoints = new List<PointF>();
                        this._widenedPointsZ = new List<PointF>();

                        if (rev)
                        {
                            for (int i = 0; i < p.Length; i++)
                            {
                                if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                                    return;
                                PointF pt = GetWidenedPointRev(p, i, (float)(35 / (double)this.HelplineRulerCtrl1.Zoom));
                                this._widenedPoints.Add(pt);
                                this._widenedPointsZ.Add(WPZTransLatePointToZoom(pt));

                                if (i % 200 == 0 && bgw != null)
                                    bgw.ReportProgress(Math.Max(Math.Min((int)(i * f), 100), 0));
                            }
                        }
                        else
                            for (int i = 0; i < p.Length; i++)
                            {
                                if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                                    return;
                                PointF pt = GetWidenedPoint(p, i, (float)(35 / (double)this.HelplineRulerCtrl1.Zoom));
                                this._widenedPoints.Add(pt);
                                this._widenedPointsZ.Add(WPZTransLatePointToZoom(pt));

                                if (i % 200 == 0 && bgw != null)
                                    bgw.ReportProgress(Math.Max(Math.Min(System.Convert.ToInt32(i * f), 100), 0));
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private PointF WPZTransLatePointToZoom(PointF pt)
        {
            if (this.HelplineRulerCtrl1.Bmp != null && this.CurPath != null && this.CurPath.Count == 1)
            {
                if (this._widenedPointsZ != null)
                    return new PointF(pt.X * this.HelplineRulerCtrl1.Zoom, pt.Y * this.HelplineRulerCtrl1.Zoom);
            }

            return new PointF(-1, -1);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.Timer1.Stop();

            TranslatePathToZoom(this.CurPath);
            PrepareWorker();
            this.BackgroundWorker1.RunWorkerAsync();
            // GetWidenedPoints()

            this.Button16.Enabled = true;
            this.Button21.Enabled = true;
            this.Button23.Enabled = true;
            this.Button22.Enabled = true;
            this.Button6.Enabled = true;

            this.ComboBox8.SelectedIndex = 0;
            this.ComboBox7.SelectedIndex = 0;

            SetUpListBox();

            CopyPath();
            this._cloneCurPath = false;

            this.Enabled = true;
            this.Button9.Enabled = false;
        }

        private void PrepareWorker()
        {
            if (this.BackgroundWorker1.IsBusy)
                this.BackgroundWorker1.CancelAsync();
            int i = 0;
            while (this.BackgroundWorker1.IsBusy && i < 50)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
                i += 1;
            }
            this.CheckBox18.Enabled = false;
            this.CheckBox1.Enabled = false;
            this.CheckBox2.Enabled = false;
            this.NumericUpDown1.Enabled = false;
            this.NumericUpDown2.Enabled = false;
            this.Button3.Enabled = false;
            this.Button4.Enabled = false;
            this.Button5.Enabled = false;
            this.ToolStripStatusLabel3.Visible = true;
            this.ToolStripProgressBar1.Visible = true;
            this.ToolStripProgressBar1.Value = this.ToolStripProgressBar1.Minimum;
        }

        private void CopyPath()
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();

            if (CurPath != null && CurPath.Count > 0)
            {
                for (int i = 0; i <= CurPath.Count - 1; i++)
                {
                    List<PointF> p = CurPath[i];

                    if (_cloneCurPath)
                    {
                        PointF[] l = new PointF[p.Count - 1 + 1];
                        p.CopyTo(l);
                        pathCopy.Add(l.ToList());
                    }
                }

                if (_cloneCurPath)
                {
                    _curPathCopy = pathCopy;
                    _cloneCurPath = false;
                    this.Button9.Enabled = true;
                }
            }

            _cloneCurPath = false;
        }

        private List<List<PointF>>? CopyPath(List<List<PointF>> curPathCopy)
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();

            if (curPathCopy != null && curPathCopy.Count() > 0)
            {
                for (int i = 0; i <= curPathCopy.Count() - 1; i++)
                {
                    List<PointF> p = curPathCopy[i];
                    PointF[] l = new PointF[p.Count - 1 + 1];
                    p.CopyTo(l);
                    pathCopy.Add(l.ToList());
                }

                return pathCopy;
            }

            return null;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                using (frmAddInsertPoint frm = new frmAddInsertPoint())
                {
                    if (!this.ListBox1.IsDisposed && this.ListBox1.SelectedIndex > -1)
                        frm.NumericUpDown1.Value = System.Convert.ToDecimal(this.ListBox1.SelectedIndex);

                    if (this._frm4RB)
                        frm.RadioButton1.Checked = true;
                    else
                        frm.RadioButton2.Checked = true;

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        float x = System.Convert.ToSingle(this.NumericUpDown1.Value);
                        float y = System.Convert.ToSingle(this.NumericUpDown2.Value);

                        this._frm4RB = frm.RadioButton1.Checked;

                        int indx = System.Convert.ToInt32(frm.NumericUpDown1.Value);
                        if (indx > this.ListBox1.Items.Count - 1)
                            indx = this.ListBox1.Items.Count - 1;

                        if (frm.RadioButton2.Checked && indx > -1 && this._listCorrespondingPts != null)
                        {
                            this.ListBox1.Items.Insert(indx, "X: " + x + "; Y: " + y);
                            this.CurPath[0].Insert(indx, new PointF(x, y));
                            this._listCorrespondingPts.Insert(indx, new PointF(x, y));
                            CPZInsertPathPointToZoom(indx, new PointF(x, y));
                            CPZTranslatePathPointToZoom(indx, new PointF(x, y));
                            PrepareWorker();
                            this.BackgroundWorker1.RunWorkerAsync();
                            // GetWidenedPoints()
                            this.ListBox1.SelectedIndex = indx;
                        }
                        else if (this._listCorrespondingPts != null)
                        {
                            this.ListBox1.Items.Add("X: " + x + "; Y: " + y);
                            this.CurPath[0].Add(new PointF(x, y));
                            this._listCorrespondingPts.Add(new PointF(x, y));
                            CPZTranslatePathPointToZoom(this.ListBox1.Items.Count - 1, new PointF(x, y));
                            PrepareWorker();
                            this.BackgroundWorker1.RunWorkerAsync();
                            // GetWidenedPoints()
                            this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;
                        }

                        this.Button9.Enabled = true;
                    }
                }
            }
        }

        private void CPZInsertPathPointToZoom(int selectedIndex, PointF p)
        {
            if (this.HelplineRulerCtrl1.Bmp != null && this.CurPath != null && this.CurPath.Count == 1)
            {
                if (this.CurPathZ != null && this.CurPathZ.Count == 1 && this.CurPathZ[0].Count > selectedIndex)
                    this.CurPathZ[0].Insert(selectedIndex, new PointF(p.X * this.HelplineRulerCtrl1.Zoom, p.Y * this.HelplineRulerCtrl1.Zoom));
                else if (this.CurPathZ != null && this.CurPathZ.Count == 1)
                    this.CurPathZ[0].Add(new PointF(p.X * this.HelplineRulerCtrl1.Zoom, p.Y * this.HelplineRulerCtrl1.Zoom));
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                if (!this.ListBox1.IsDisposed && this.ListBox1.SelectedIndex > -1)
                {
                    int indx = this.ListBox1.SelectedIndex;

                    if (this.cbRemRnge.Checked)
                    {
                        this._crRemoveRange = this.cbRemRnge.Checked;
                        this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                        if (this._frmRR == null)
                        {
                            this._frmRR = new frmRemoveRange(this, indx, this._selctedStPtIndex, this._selctedEPtIndex);
                            this._frmRR.NumsChanged += _frmRR_NumsChanged;
                            this._frmRR.RemovePoints += _frmRR_RemovePoints;
                        }

                        this._frmRR.StartIndex = indx;
                        this._frmRR.EndIndex = indx;
                        this._frmRR.numStIndx.Value = (decimal)indx;
                        this._frmRR.numEIndx.Value = (decimal)indx;

                        if (this._frmRR.ShowDialog() == DialogResult.OK)
                        {
                            this._selctedStPtIndex = this._frmRR.StartIndex;
                            this._selctedEPtIndex = this._frmRR.EndIndex;
                        }

                        this._crRemoveRange = false;
                        this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                    else if (this._listCorrespondingPts != null && this.CurPathZ != null && this._widenedPoints != null && this._widenedPointsZ != null)
                    {
                        this.ListBox1.Items.RemoveAt(indx);
                        this.CurPath[0].RemoveAt(indx);
                        this._listCorrespondingPts.RemoveAt(indx);
                        this.CurPathZ[0].RemoveAt(indx);
                        this._widenedPoints.RemoveAt(indx);
                        this._widenedPointsZ.RemoveAt(indx);

                        if (indx < this.ListBox1.Items.Count)
                            this.ListBox1.SelectedIndex = indx;
                        else
                            this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;

                        this.Button9.Enabled = true;
                        this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }
        }

        private void _frmRR_RemovePoints(object? sender, IntegerEventArgs4 e)
        {
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                if (!this.ListBox1.IsDisposed && this.ListBox1.SelectedIndex > -1)
                {
                    if (e.Start >= 0 && e.End >= 0 && e.Start < CurPath[0].Count && e.End < CurPath[0].Count)
                    {
                        int indx = -1;
                        bool addClosingPoint = false;

                        int st = e.Start;
                        int ed = e.End;
                        int cnt = this.CurPath[0].Count;
                        if (this.CurPath[0] != null && this.CurPath[0].Count > 1 && this._listCorrespondingPts != null && this.CurPathZ != null && this._widenedPoints != null && this._widenedPointsZ != null)
                        {
                            if (this._selctedStPtIndex > this._selctedEPtIndex)
                            {
                                ed += cnt;
                                addClosingPoint = true;
                            }

                            for (int i = ed - 1; i > st; i--)
                            {
                                indx = i % cnt;
                                if (indx >= this.CurPath[0].Count)
                                    indx = this.CurPath[0].Count - 1;
                                this.ListBox1.Items.RemoveAt(indx);
                                this.CurPath[0].RemoveAt(indx);
                                this._listCorrespondingPts.RemoveAt(indx);
                                this.CurPathZ[0].RemoveAt(indx);
                                this._widenedPoints.RemoveAt(indx);
                                this._widenedPointsZ.RemoveAt(indx);
                            }

                            if (addClosingPoint)
                            {
                                int x = (int)this.CurPath[0][0].X;
                                int y = (int)this.CurPath[0][0].Y;
                                this.ListBox1.Items.Add("X: " + x + "; Y: " + y);
                                this.CurPath[0].Add(new PointF(x, y));
                                this._listCorrespondingPts.Add(new PointF(x, y));
                                CPZTranslatePathPointToZoom(this.ListBox1.Items.Count - 1, new PointF(x, y));
                                PrepareWorker();
                                this.BackgroundWorker1.RunWorkerAsync();
                                // GetWidenedPoints()
                                //this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;
                            }
                        }

                        if (indx < this.ListBox1.Items.Count)
                            this.ListBox1.SelectedIndex = indx;
                        else
                            this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;

                        this.Button9.Enabled = true;
                        this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }
        }

        private void _frmRR_NumsChanged(object? sender, IntegerEventArgs4 e)
        {
            if (this.CurPath != null && e.Start >= 0 && e.End >= 0 && e.Start < CurPath[0].Count && e.End < CurPath[0].Count)
            {
                this._selctedStPtIndex = e.Start;
                this._selctedEPtIndex = e.End;
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.HelplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.HelplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (CurPath != null && CurPath.Count > 0 && _bmpBU != null)
            {
                this.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();
                using (GraphicsPath? gp = GetPath(this.CurPath))
                {
                    if (gp != null)
                        using (frmDisplayImg frm = new frmDisplayImg(_bmpBU, gp))
                        {
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                float scaleX = System.Convert.ToSingle(frm.NumericUpDown2.Value);
                                float scaleY = System.Convert.ToSingle(frm.NumericUpDown3.Value);
                                float shiftX = System.Convert.ToSingle(frm.NumericUpDown4.Value);
                                float shiftY = System.Convert.ToSingle(frm.NumericUpDown5.Value);

                                RectangleF r = gp.GetBounds();
                                using (Matrix mx = new Matrix(1, 0, 0, 1, (float)(-r.X - (r.Width / (double)2.0F)), (float)(-r.Y - (r.Height / (double)2.0F))))
                                {
                                    gp.Transform(mx);
                                }
                                using (Matrix mx = new Matrix(scaleX, 0, 0, scaleY, 0, 0))
                                {
                                    gp.Transform(mx);
                                }
                                // Dim r2 As RectangleF = gp.GetBounds()
                                using (Matrix mx = new Matrix(1, 0, 0, 1, (float)(r.X + (r.Width / (double)2.0F) + shiftX), (float)(r.Y + (r.Height / (double)2.0F) + shiftY)))
                                {
                                    gp.Transform(mx);
                                }

                                if (gp.PointCount > 1)
                                {
                                    PointF[] p = new PointF[gp.PathPoints.Length];
                                    gp.PathPoints.CopyTo(p, 0);
                                    PointF[] pts = p;

                                    this.CurPath = new List<List<PointF>>();
                                    this.CurPath.Add(pts.ToList());

                                    TranslatePathToZoom(this.CurPath);
                                    PrepareWorker();
                                    this.BackgroundWorker1.RunWorkerAsync();
                                    // GetWidenedPoints()

                                    this.Button9.Enabled = true;
                                    SetUpListBox();
                                }
                                else
                                    MessageBox.Show("Path is empty");
                            }
                        }
                }
                this.Enabled = true;
                this.Cursor = Cursors.Default;
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private GraphicsPath? GetPath(List<List<PointF>>? curPath)
        {
            if (curPath != null && curPath.Count > 0)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (curPath != null && curPath.Count > 0)
                    {
                        for (int i = 0; i <= curPath.Count - 1; i++)
                        {
                            List<PointF> p = curPath[i];
                            if (p != null && p.Count > 1)
                                gp.AddLines(p.ToArray());
                        }
                    }

                    return (GraphicsPath)gp.Clone();
                }
            }

            return null;
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            GetWidenedPoints(this.BackgroundWorker1);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (this._widenedPoints != null && this._widenedPoints.Count < 1000)
                this.CheckBox18.Enabled = true;
            else
            {
                this.CheckBox18.Checked = false;
                this.CheckBox18.Enabled = false;
            }
            this.CheckBox1.Enabled = true;
            this.CheckBox2.Enabled = true;
            this.NumericUpDown1.Enabled = true;
            this.NumericUpDown2.Enabled = true;
            this.Button3.Enabled = true;
            this.Button4.Enabled = true;
            this.Button5.Enabled = true;
            this.ToolStripStatusLabel3.Visible = false;
            this.ToolStripProgressBar1.Visible = false;

            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (this.Visible && this.ToolStripProgressBar1.IsDisposed == false && this.BackgroundWorker1.IsBusy)
                this.ToolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void frmEditPath_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.BackgroundWorker1.IsBusy)
                this.BackgroundWorker1.CancelAsync();
            int i = 0;
            while (this.BackgroundWorker1.IsBusy && i < 50)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
                i += 1;
            }
        }

        private void Add100PxBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.BackgroundWorker1.IsBusy)
                this.BackgroundWorker1.CancelAsync();

            Bitmap? b = null;
            if (this._bmpBU != null && this.HelplineRulerCtrl1.Bmp != null && this.BackgroundWorker1.IsBusy == false)
            {
                if (AvailMem.AvailMem.checkAvailRam((this._bmpBU.Width + 200) * (this._bmpBU.Height + 200) * 12L))
                    b = new Bitmap(this.HelplineRulerCtrl1.Bmp.Width + 200, this.HelplineRulerCtrl1.Bmp.Height + 200);
                else
                    return;

                using (Graphics gx = Graphics.FromImage(b))
                {
                    gx.DrawImage(this.HelplineRulerCtrl1.Bmp, 100, 100);
                }

                this.SetBitmap(this.HelplineRulerCtrl1.Bmp, b, this.HelplineRulerCtrl1, "Bmp");

                double faktor = System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Height);
                double multiplier = System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height);
                if (multiplier >= faktor)
                    this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width));
                else
                    this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height));
                //this._zoomWidth = false;

                this.HelplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Height * this.HelplineRulerCtrl1.Zoom));

                this.HelplineRulerCtrl1.MakeBitmap(this.HelplineRulerCtrl1.Bmp);

                this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                this._dontDoZoom = true;
                this.ComboBox2.SelectedIndex = 4;
                this._dontDoZoom = false;

                this._bmpWidth = this.HelplineRulerCtrl1.Bmp.Width;
                this._bmpHeight = this.HelplineRulerCtrl1.Bmp.Height;

                TranslatePathAndWidenedPoints(100);
            }
        }

        private void Remove100PxBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.BackgroundWorker1.IsBusy)
                this.BackgroundWorker1.CancelAsync();

            Bitmap? b = null;
            if (this._bmpBU != null && this.HelplineRulerCtrl1.Bmp != null && this.BackgroundWorker1.IsBusy == false)
            {
                if (AvailMem.AvailMem.checkAvailRam((this._bmpBU.Width) * (this._bmpBU.Height) * 12L))
                    b = new Bitmap(this._bmpBU.Width, this._bmpBU.Height);
                else
                    return;

                using (Graphics gx = Graphics.FromImage(b))
                {
                    gx.DrawImage(this.HelplineRulerCtrl1.Bmp, -100, -100);
                }

                this.SetBitmap(this.HelplineRulerCtrl1.Bmp, b, this.HelplineRulerCtrl1, "Bmp");

                double faktor = System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Height);
                double multiplier = System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height);
                if (multiplier >= faktor)
                    this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width));
                else
                    this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.HelplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height));
                //this._zoomWidth = false;

                this.HelplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Height * this.HelplineRulerCtrl1.Zoom));

                this.HelplineRulerCtrl1.MakeBitmap(this.HelplineRulerCtrl1.Bmp);

                this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                this._dontDoZoom = true;
                this.ComboBox2.SelectedIndex = 4;
                this._dontDoZoom = false;

                this._bmpWidth = this.HelplineRulerCtrl1.Bmp.Width;
                this._bmpHeight = this.HelplineRulerCtrl1.Bmp.Height;

                TranslatePathAndWidenedPoints(-100);
            }
        }

        private void TranslatePathAndWidenedPoints(int wh)
        {
            this.Enabled = false;

            TranslateCurPath(wh);

            TranslatePathToZoom(this.CurPath);
            PrepareWorker();
            this.BackgroundWorker1.RunWorkerAsync();
            // GetWidenedPoints()

            this.Button16.Enabled = true;
            this.Button21.Enabled = true;
            this.Button23.Enabled = true;
            this.Button22.Enabled = true;
            this.Button6.Enabled = true;

            this.ComboBox8.SelectedIndex = 0;
            this.ComboBox7.SelectedIndex = 0;

            SetUpListBox();

            CopyPath();
            this._cloneCurPath = false;

            this.Enabled = true;
            this.Button9.Enabled = false;
        }

        private void TranslateCurPath(int wh)
        {
            if (this.CurPath != null && this.CurPath.Count > 0 && this.CurPath[0].Count > 0)
            {
                for (int i = 0; i <= CurPath.Count - 1; i++)
                {
                    List<PointF> p = CurPath[i];

                    for (int j = 0; j <= p.Count - 1; j++)
                        p[j] = new PointF(p[j].X + wh, p[j].Y + wh);
                }

                this._borderSum += wh;
                this.Remove100PxBorderToolStripMenuItem.Enabled = (this._borderSum > 0);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (this._borderSum != 0)
                TranslateCurPath(-this._borderSum);
        }

        private void ToolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                GraphicsPath? gPath = null;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (this.CurPath.Count > 0)
                    {
                        for (int i = 0; i <= this.CurPath.Count - 1; i++)
                        {
                            List<PointF> p = this.CurPath[i];
                            if (p != null && p.Count > 1)
                                gp.AddLines(p.ToArray());
                        }
                        if (gp.PointCount > 1)
                        {
                            gPath = new GraphicsPath();
                            gPath.AddPath((GraphicsPath)gp.Clone(), false);
                        }
                    }
                }

                if (gPath != null && gPath.PointCount > 1)
                {
                    if (this.SaveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            JsonSerializerOptions options = new()
                            {
                                NumberHandling =
                                    JsonNumberHandling.AllowReadingFromString |
                                    JsonNumberHandling.WriteAsString,
                                WriteIndented = true,
                                ReadCommentHandling = JsonCommentHandling.Skip,
                                AllowTrailingCommas = true,
                            };

                            object[] o = new object[] { gPath.PathPoints, gPath.PathTypes };
                            string FileName = this.SaveFileDialog2.FileName;

                            using FileStream createStream = File.Create(FileName);
                            JsonSerializer.Serialize(createStream, o, options);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                        }
                    }
                }
            }
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png|All Files - (*.*)|*.*";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;

                try
                {
                    using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    {
                        if (AvailMem.AvailMem.checkAvailRam(img.Width * img.Height * 4L))
                            bmp = (Bitmap)img.Clone();
                        else
                            throw new Exception();
                    }

                    this.SetBitmap(this.HelplineRulerCtrl1.Bmp, bmp, this.HelplineRulerCtrl1, "Bmp");

                    this.HelplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Height * this.HelplineRulerCtrl1.Zoom));
                    this.HelplineRulerCtrl1.MakeBitmap(this.HelplineRulerCtrl1.Bmp);
                    this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                    this.Text = this.openFileDialog1.FileName + " - frmQuickExtract";

                    this._bmpWidth = this.HelplineRulerCtrl1.Bmp.Width;
                    this._bmpHeight = this.HelplineRulerCtrl1.Bmp.Height;

                    this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                }
                catch
                {
                    if (bmp != null)
                        bmp.Dispose();

                    MessageBox.Show("Not a valid image file. Maybe its an encrypted file.");
                }
            }
        }
    }
}
