using Cache;
using ChainCodeFinder;
using QuickExtractingLib2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickExtract2
{
    public partial class frmSetAndEditSeedPoints : Form
    {
        private Bitmap? _bmpBU;

        private Color _zColor = Color.Yellow;
        private Color _fColor = Color.Orange;
        private Color _ffColor = Color.Red;
        private bool _dontDoZoom;

        public List<List<List<PointF>>>? PathList { get; private set; }
        public List<List<List<PointF>>>? PathListNew { get; private set; }

        private bool _drawPath;
        private int _selctedIndex;
        private int _ix;
        private int _iy;
        private int _foundIndex;
        private PointF _offset;
        //private int _curSeedPoint;
        private bool _dontDoMouseMove;
        private bool firstClick = true;
        private PointF _tempLast;
        private bool _pathClosed;
        private bool _reRunLineRunPrev;
        private PointF _spBU;
        private int _curPos;
        private bool _drawPathPart;
        private bool _finished;
        private bool _computeFullPath;
        private List<List<List<PointF>>>? _pathListBU;

        public QuickExtractingCtrl? QuickExtractingCtrl { get; internal set; }
        public Bitmap? BmpForValueComputation { get; internal set; }
        public Bitmap? ImgDataPic { get; internal set; }
        public List<PointF>? SeedPoints { get; private set; }
        public List<PointF>? SeedPointsZ { get; private set; }
        public List<PointF>? SeedPointsBU { get; private set; }

        public frmSetAndEditSeedPoints(Bitmap bmp, QuickExtractingCtrl quickExtractingCtrl1)
        {
            InitializeComponent();
            this.QuickExtractingCtrl = quickExtractingCtrl1;

            if (bmp != null)
            {
                this.helplineRulerCtrl1.Bmp = new Bitmap(bmp);
                this._bmpBU = new Bitmap(bmp);

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

                this.helplineRulerCtrl1.dbPanel1.MouseDown += Helplinerulerctrl1_MouseDown;
                this.helplineRulerCtrl1.dbPanel1.MouseMove += Helplinerulerctrl1_MouseMove;
                this.helplineRulerCtrl1.dbPanel1.MouseUp += Helplinerulerctrl1_MouseUp;
                this.helplineRulerCtrl1.PostPaint += Helplinerulerctrl1_Paint;
            }
        }

        private void Helplinerulerctrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            this._foundIndex = -1;
            if (e.Button == MouseButtons.Left && this.SeedPoints != null && this.helplineRulerCtrl1.Bmp != null && this.CheckBox1.Checked)
            {
                _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                bool found = false;
                int indx = -1;

                if (this.SeedPointsZ != null)
                {
                    Point asp = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition;
                    for (int j = this.SeedPointsZ.Count - 1; j >= 0; j--)
                    {
                        if (new RectangleF(this.SeedPointsZ[j].X - 16 + asp.X,
                                this.SeedPointsZ[j].Y - 16 + asp.Y, 32, 32).Contains(new PointF(e.X, e.Y)))
                        {
                            found = true;
                            indx = j;
                            this._offset = new PointF(this.SeedPointsZ[j].X - e.X, this.SeedPointsZ[j].Y - e.Y);
                            break;
                        }
                    }
                }

                if (found)
                {
                    this._foundIndex = indx;
                    this.ListBox1.SelectedIndex = indx;
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }
        private void Helplinerulerctrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!this._dontDoMouseMove)
            {
                if (e.Button == MouseButtons.Left && this._foundIndex > -1 && this.SeedPointsZ != null && this.SeedPointsZ.Count > this._foundIndex)
                {
                    this.SeedPointsZ[this._foundIndex] = new PointF(e.X + this._offset.X, e.Y + this._offset.Y);
                    if (this.SeedPoints != null)
                    {
                        this.SeedPoints[this._foundIndex] = new PointF((int)(this.SeedPointsZ[this._foundIndex].X / this.helplineRulerCtrl1.Zoom),
                            (int)(this.SeedPointsZ[this._foundIndex].Y / this.helplineRulerCtrl1.Zoom));
                        this.ListBox1.Items[this._foundIndex] = this.SeedPoints[this._foundIndex].ToString();
                        this.ListBox1.SelectedIndex = this._foundIndex;
                    }

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private void Helplinerulerctrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.ListBox1.SelectedIndex == -1)
                this._foundIndex = -1;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Helplinerulerctrl1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            if (this._drawPath || this._drawPathPart)
            {
                using (GraphicsPath gp = new GraphicsPath())
                //using (GraphicsPath gp2 = new GraphicsPath())
                {
                    if (this.QuickExtractingCtrl?.CurPath != null && this.QuickExtractingCtrl.CurPath.Count > 0)
                    {
                        for (int i = 0; i < this.QuickExtractingCtrl.CurPath.Count; i++)
                        {
                            List<PointF> p = this.QuickExtractingCtrl.CurPath[i];
                            if (p != null && p.Count > 1)
                            {
                                gp.AddLines(p.ToArray());
                                //if (this._reRunLineRunPrev && i == this.QuickExtractingCtrl.CurPath.Count - 1)
                                //    gp2.AddLines(p.ToArray());
                            }
                        }
                    }
                    float w = System.Convert.ToSingle(this.numPenWidth.Value);
                    int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                    int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                    using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                    {
                        gp.Transform(m);
                        using (Pen pen = new Pen(new SolidBrush(this._fColor), w))
                        {
                            e.Graphics.DrawPath(pen, gp);
                            this.label9.Text = (gp.PointCount).ToString() + " - points";
                        }

                        //gp2.Transform(m);
                        //using (Pen pen = new Pen(new SolidBrush(this._ffColor), w))
                        //    e.Graphics.DrawPath(pen, gp2);
                    }
                }
            }

            if (!this._drawPath)
            {
                if (this.SeedPoints != null && this.SeedPoints.Count > 0)
                {
                    Point asp = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition;

                    if (this.SeedPointsZ != null)
                        for (int j = 0; j < this.SeedPointsZ.Count; j++)
                        {
                            if (this._foundIndex == -1 || j != this._foundIndex)
                            {
                                using SolidBrush sb = new SolidBrush(this._ffColor);
                                using Pen pen = new Pen(sb, 2 /*Math.Max((float)((int)this.numPenWidth.Value * this.helplineRulerCtrl1.Zoom), 1)*/);
                                e.Graphics.DrawRectangle(pen, new RectangleF(this.SeedPointsZ[j].X - 16 + asp.X,
                                    this.SeedPointsZ[j].Y - 16 + asp.Y, 32, 32));
                            }
                            else
                            {
                                using SolidBrush sb = new SolidBrush(this._fColor);
                                using Pen pen = new Pen(sb, 2 /*Math.Max((float)((int)this.numPenWidth.Value * this.helplineRulerCtrl1.Zoom), 1)*/);
                                e.Graphics.DrawRectangle(pen, new RectangleF(this.SeedPointsZ[j].X - 16 + asp.X,
                                    this.SeedPointsZ[j].Y - 16 + asp.Y, 32, 32));
                            }
                            e.Graphics.FillRectangle(Brushes.Lime, new RectangleF(this.SeedPointsZ[j].X - 1 + asp.X,
                                this.SeedPointsZ[j].Y - 1 + asp.Y, 3, 3));
                        }
                }
            }
        }

        private void frmSetAndEditSeedPoints_Load(object sender, EventArgs e)
        {
            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.PathList != null && this.QuickExtractingCtrl.PathList.Count > 0)
            {
                for (int i = 0; i < this.QuickExtractingCtrl.PathList.Count; i++)
                    this.cmbPaths.Items.Add("SavedPath_" + i.ToString() + "; " + this.QuickExtractingCtrl.PathList[i][0].Count.ToString());
                this._pathListBU = CloneList(this.QuickExtractingCtrl.PathList);
                this.cmbPaths.SelectedIndex = 0;

                this._dontDoZoom = true;
                this.ComboBox2.SelectedIndex = 4;
                this._dontDoZoom = false;

                this.PathList = CloneList(this.QuickExtractingCtrl.PathList);
                this.PathListNew = new List<List<List<PointF>>>();

                this.numValL.Value = this.QuickExtractingCtrl.numValL.Value;
                this.numLapTh.Value = this.QuickExtractingCtrl.numLapTh.Value;
                this.numValM.Value = this.QuickExtractingCtrl.numValM.Value;
                this.numValG.Value = this.QuickExtractingCtrl.numValG.Value;
                this.numEdgeWeight.Value = this.QuickExtractingCtrl.numEdgeWeight.Value;
                this.numValCl.Value = this.QuickExtractingCtrl.numValCl.Value;
                this.numValC0l.Value = this.QuickExtractingCtrl.numValC0l.Value;

                this.cbScaleValues.Checked = this.QuickExtractingCtrl.cbScaleValues.Checked;
                this.cbAddLine.Checked = this.QuickExtractingCtrl.cbAddLine.Checked;
                this.cbAutoAddLine.Checked = this.QuickExtractingCtrl.cbAutoAddLine.Checked;
                this.cbUseCostMaps.Checked = this.QuickExtractingCtrl.cbUseCostMaps.Checked;

                this.numValP.Value = this.QuickExtractingCtrl.numValP.Value;
                this.numVal_I.Value = this.QuickExtractingCtrl.numVal_I.Value;
                this.numValO.Value = this.QuickExtractingCtrl.numValO.Value;
            }
            else if (this.QuickExtractingCtrl != null)
            {
                this.QuickExtractingCtrl.PathList = new List<List<List<PointF>>>();
                this.QuickExtractingCtrl.PathList.Add(new List<List<PointF>>());
                this.QuickExtractingCtrl.PathList[0].Add(new List<PointF>());
                this.QuickExtractingCtrl.PathList[0][0].Add(new PointF((this.helplineRulerCtrl1.dbPanel1.ClientSize.Width - 32) / this.helplineRulerCtrl1.Zoom,
                        32 / this.helplineRulerCtrl1.Zoom));
                int z = this.helplineRulerCtrl1.dbPanel1.ClientSize.Width - 128;
                for (int i = 0; i < z; i++)
                    this.QuickExtractingCtrl.PathList[0][0].Add(new PointF(
                        (this.helplineRulerCtrl1.dbPanel1.ClientSize.Width - 32 - i) / this.helplineRulerCtrl1.Zoom, 32 / this.helplineRulerCtrl1.Zoom));
                this.QuickExtractingCtrl.PathList[0][0].Add(new PointF(32 / this.helplineRulerCtrl1.Zoom, 32 / this.helplineRulerCtrl1.Zoom));

                this.cmbPaths.Items.Add("SavedPath_" + 0.ToString() + "; " + this.QuickExtractingCtrl.PathList[0][0].Count.ToString());
                this._pathListBU = CloneList(this.QuickExtractingCtrl.PathList);
                this.cmbPaths.SelectedIndex = 0;

                this._dontDoZoom = true;
                this.ComboBox2.SelectedIndex = 4;
                this._dontDoZoom = false;

                this.PathList = CloneList(this.QuickExtractingCtrl.PathList);
                this.PathListNew = new List<List<List<PointF>>>();

                this.numValL.Value = this.QuickExtractingCtrl.numValL.Value;
                this.numLapTh.Value = this.QuickExtractingCtrl.numLapTh.Value;
                this.numValM.Value = this.QuickExtractingCtrl.numValM.Value;
                this.numValG.Value = this.QuickExtractingCtrl.numValG.Value;
                this.numEdgeWeight.Value = this.QuickExtractingCtrl.numEdgeWeight.Value;
                this.numValCl.Value = this.QuickExtractingCtrl.numValCl.Value;
                this.numValC0l.Value = this.QuickExtractingCtrl.numValC0l.Value;

                this.cbScaleValues.Checked = this.QuickExtractingCtrl.cbScaleValues.Checked;
                this.cbAddLine.Checked = this.QuickExtractingCtrl.cbAddLine.Checked;
                this.cbAutoAddLine.Checked = this.QuickExtractingCtrl.cbAutoAddLine.Checked;
                this.cbUseCostMaps.Checked = this.QuickExtractingCtrl.cbUseCostMaps.Checked;

                this.numValP.Value = this.QuickExtractingCtrl.numValP.Value;
                this.numVal_I.Value = this.QuickExtractingCtrl.numVal_I.Value;
                this.numValO.Value = this.QuickExtractingCtrl.numValO.Value;
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

        private List<List<PointF>>? CloneList(List<List<PointF>>? pathList)
        {
            if (pathList != null && pathList.Count > 0)
            {
                List<List<PointF>> lOut = new List<List<PointF>>();
                for (int i = 0; i < pathList.Count; i++)
                {
                    List<PointF> p = pathList[i];
                    List<PointF> l1 = new List<PointF>();

                    for (int j = 0; j < p.Count; j++)
                    {
                        PointF q = p[j];
                        PointF l2 = q;
                        l1.Add(l2);
                    }

                    lOut.Add(l1);
                }

                return lOut;
            }

            return null;
        }

        private void frmSetAndEditSeedPoints_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._bmpBU != null)
                this._bmpBU.Dispose();

            if (this.QuickExtractingCtrl?.Alg != null)
            {
                //this.QuickExtractingCtrl.Alg.NotifyEdges -= Alg_NotifyEdges;
                this.QuickExtractingCtrl.Alg.Dispose();
            }
        }

        private void btnComputePath_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                    this.QuickExtractingCtrl?.Alg.UnlockBmpData();

                if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.CurPath != null &&
                    this.QuickExtractingCtrl.CurPath.Count > 0 && this.QuickExtractingCtrl.CurPath[0].Count > 0 &&
                    this.BmpForValueComputation != null && this.ImgDataPic != null)
                {
                    this._computeFullPath = true;
                    this.button6_Click(this.btnComputeStep, new EventArgs());
                }
                else if(this.QuickExtractingCtrl?.CurPath?.Count == 0)
                {
                    this._computeFullPath = true;
                    this.button6_Click(this.btnComputeStep, new EventArgs());
                }
            }
        }

        private void cmbPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.PathList != null && this.QuickExtractingCtrl.PathList.Count > this.cmbPaths.SelectedIndex)
                this.QuickExtractingCtrl.CurPath = this.QuickExtractingCtrl.PathList[this.cmbPaths.SelectedIndex];

            this._drawPath = true;

            if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                this.QuickExtractingCtrl?.Alg.UnlockBmpData();

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnGetSeedPts_Click(object sender, EventArgs e)
        {
            this.SeedPoints = new List<PointF>();
            int amount = (int)this.numAmntSP.Value;

            if (this.QuickExtractingCtrl != null)
            {
                if (this.QuickExtractingCtrl.CurPath != null && this.QuickExtractingCtrl.CurPath.Count > 0 && this.QuickExtractingCtrl.CurPath[0].Count > 0)
                {
                    double d = (double)this.QuickExtractingCtrl.CurPath[0].Count / (double)amount;
                    int cnt = 0;
                    int cnt2 = 0;
                    double c = 0;

                    this.ListBox1.Items.Clear();

                    this.ListBox1.BeginUpdate();

                    while (cnt < this.QuickExtractingCtrl.CurPath[0].Count && cnt2 < amount)
                    {
                        this.SeedPoints.Add(this.QuickExtractingCtrl.CurPath[0][cnt]);
                        this.ListBox1.Items.Add(this.QuickExtractingCtrl.CurPath[0][cnt]);
                        c += d;
                        cnt = (int)c;
                        cnt2++;
                    }

                    this.ListBox1.EndUpdate();

                    this.SeedPointsZ = GetTransformedSP(this.SeedPoints);
                    this._drawPath = false;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.firstClick = true;
                    this._curPos = 0;

                    this.btnBackupSP.Enabled = true;
                }
            }
        }

        private List<PointF> GetTransformedSP(List<PointF> seedPoints)
        {
            List<PointF> l = new List<PointF>();

            if (this.SeedPoints != null)
                for (int j = 0; j < this.SeedPoints?.Count; j++)
                    l.Add(new PointF(this.SeedPoints[j].X * this.helplineRulerCtrl1.Zoom, this.SeedPoints[j].Y * this.helplineRulerCtrl1.Zoom));

            return l;
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            if (this.SeedPoints != null)
                this.SeedPointsZ = GetTransformedSP(this.SeedPoints);

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(ComboBox2.SelectedItem?.ToString());
                this.helplineRulerCtrl1.Enabled = true;
                if (this.ComboBox2.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                if (this.SeedPoints != null)
                    this.SeedPointsZ = GetTransformedSP(this.SeedPoints);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.ListBox1.IsDisposed && this.ListBox1.SelectedIndex > -1)
            {
                this.Label2.Text = "pt " + this.ListBox1.SelectedIndex + " of " + this.SeedPoints?.Count.ToString();

                this.NumericUpDown1.Value = System.Convert.ToDecimal(this.SeedPoints?[this.ListBox1.SelectedIndex].X);
                this.NumericUpDown2.Value = System.Convert.ToDecimal(this.SeedPoints?[this.ListBox1.SelectedIndex].Y);

                this._selctedIndex = this._foundIndex = this.ListBox1.SelectedIndex;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (this.SeedPoints != null && this.ListBox1.SelectedIndex > -1)
            {
                if (this.SeedPoints.Count > this.ListBox1.SelectedIndex)
                {
                    int j = this.ListBox1.SelectedIndex;
                    this.SeedPoints.RemoveAt(this.ListBox1.SelectedIndex);
                    this.SeedPointsZ?.RemoveAt(this.ListBox1.SelectedIndex);
                    this.ListBox1.Items.RemoveAt(this.ListBox1.SelectedIndex);

                    this.ListBox1.SelectedIndex = (this.ListBox1.Items.Count > j) ? j : 0;

                    this._curPos = 0;
                    this.firstClick = true;
                    if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                        this.QuickExtractingCtrl?.Alg.UnlockBmpData();

                    this.helplineRulerCtrl1?.dbPanel1.Invalidate();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this._curPos == 0)
                this.CheckSeedPoints(this.ImgDataPic, this.SeedPoints);
            else if (this._curPos < this.SeedPoints?.Count)
                this.CheckSeedPoint(this.ImgDataPic, this.SeedPoints, this._curPos);

            this.label24.Text = "running...";
            this.btnComputeStep.Enabled = this.btnComputePath.Enabled = this.helplineRulerCtrl1.Enabled = false;
            this.btnComputeStep.Refresh();
            this.btnComputePath.Refresh();

            if (this.QuickExtractingCtrl != null && this.SeedPoints != null && this.SeedPoints?.Count > this._curPos)
            {
                this._drawPathPart = true;
                bool fc = false;

                if (this.firstClick)
                {
                    this._finished = false;
                    this.QuickExtractingCtrl.SeedPoints = new List<PointF>();
                    this.QuickExtractingCtrl.TempPath = new List<PointF>();
                    this.QuickExtractingCtrl.CurPath = new List<List<PointF>>();
                    BackupSeedPoints(this.SeedPoints);
                    this.label23.Enabled = this.btnLoadSeedPoints.Enabled = this.btnBackupSP.Enabled = true;
                    fc = true;
                }

                apply();
                this._curPos++;

                if (fc && this.SeedPoints?.Count > this._curPos)
                {
                    apply();
                    this._curPos++;
                }
            }
            else
            {
                if (!this._finished)
                {
                    this._finished = true;
                    applyClose();
                    this._curPos++;
                    //closePath();         
                }

                if (this.timer3.Enabled)
                    this.timer3.Stop();
                this.timer3.Start();
            }

            this.label25.Text = "CurPos: " + this._curPos.ToString();
        }

        private void CheckSeedPoint(Bitmap? imgDataPic, List<PointF>? seedPoints, int j)
        {
            if (imgDataPic != null && seedPoints != null)
            {
                int w = imgDataPic.Width;
                int h = imgDataPic.Height;

                if (seedPoints[j].X < 0)
                    seedPoints[j] = new PointF(0, seedPoints[j].Y);
                if (seedPoints[j].X > w - 1)
                    seedPoints[j] = new PointF(w - 1, seedPoints[j].Y);
                if (seedPoints[j].Y < 0)
                    seedPoints[j] = new PointF(seedPoints[j].X, 0);
                if (seedPoints[j].Y > h - 1)
                    seedPoints[j] = new PointF(seedPoints[j].X, h - 1);

            }
        }

        private void BackupSeedPoints(List<PointF> seedPoints)
        {
            if (seedPoints != null)
            {
                this.SeedPointsBU = new List<PointF>();

                for (int j = 0; j < seedPoints.Count; j++)
                    this.SeedPointsBU.Add(new PointF(seedPoints[j].X, seedPoints[j].Y));
            }
        }

        private void PrepareBGW(object[] args)
        {
            int num = System.Convert.ToInt32(args[0]);
            Point pt = (Point)args[1];
            QuickExtractingParameters @params = (QuickExtractingParameters)args[2];

            this.Cursor = Cursors.WaitCursor;

            cleanCP(); // checkPath here for consistency of seedpoints

            if (this.QuickExtractingCtrl != null)
            {
                switch (num)
                {
                    case 1:
                        {
                            if ((this.QuickExtractingCtrl.Alg == null || this.firstClick))
                            {
                                this.QuickExtractingCtrl.Alg = new QuickExtractingAlg(@params.imgDataPic, @params.bmpDataForValueComputation, pt, @params.doR, @params.doG, @params.doB, @params.doScale, @params.neighbors, this.backgroundWorker1);
                                this.QuickExtractingCtrl.Alg.SetColorBmp(true);
                                //this.QuickExtractingCtrl.Alg.NotifyEdges += Alg_NotifyEdges;
                            }

                            if (this.QuickExtractingCtrl.Alg.picAlg?.CostMaps == null && @params.useCostMap)
                                this.QuickExtractingCtrl.Alg.InitCostMapStandardCalc();
                            else if ((!@params.useCostMap))
                                if (this.QuickExtractingCtrl.Alg.picAlg != null)
                                    this.QuickExtractingCtrl.Alg.picAlg.CostMaps = null;

                            if (this.QuickExtractingCtrl.Alg.picAlg != null && this.QuickExtractingCtrl.Alg.picAlg.CostMaps != null && @params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0)
                                this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                            this.QuickExtractingCtrl.Alg.valL = @params.valL;
                            this.QuickExtractingCtrl.Alg.valM = @params.valM;
                            this.QuickExtractingCtrl.Alg.valG = @params.valG;
                            this.QuickExtractingCtrl.Alg.laplTh = @params.laplTh;
                            this.QuickExtractingCtrl.Alg.edgeWeight = @params.edgeWeight;
                            this.QuickExtractingCtrl.Alg.dist = @params.dist;

                            this.QuickExtractingCtrl.Alg.CurPath = @params.CurPath;
                            this.QuickExtractingCtrl.Alg.SeedPoints = @params.SeedPoints;

                            this.QuickExtractingCtrl.Alg.valP = @params.valP;
                            this.QuickExtractingCtrl.Alg.valI = @params.valI;
                            this.QuickExtractingCtrl.Alg.valO = @params.valO;

                            this.QuickExtractingCtrl.Alg.valCl = @params.valCl;
                            this.QuickExtractingCtrl.Alg.valCol = @params.valCol;

                            this.QuickExtractingCtrl.Alg.NotifyEach = @params.notifyEach;

                            if (!this.firstClick)
                            {
                                if (!this.backgroundWorker1.IsBusy && QuickExtractingCtrl.Alg.SeedPoints != null && QuickExtractingCtrl.Alg.SeedPoints.Count > 0)
                                {
                                    this.QuickExtractingCtrl.Alg.MouseClicked = @params.MouseClicked;
                                    this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint = new Point(System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].Y));
                                    this.QuickExtractingCtrl.Alg.ReInit(pt.X, pt.Y, @params.doScale, @params.neighbors);

                                    bool found = false;

                                    if (!this.QuickExtractingCtrl.Alg.MouseClicked)
                                    {
                                        for (int j = 0; j <= QuickExtractingCtrl.Alg.SeedPoints.Count - 1; j++)
                                        {
                                            if (QuickExtractingCtrl.Alg.SeedPoints[j].X == pt.X && QuickExtractingCtrl.Alg.SeedPoints[j].Y == pt.Y)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!found && QuickExtractingCtrl.Alg.MouseClicked)
                                        QuickExtractingCtrl.Alg.SeedPoints.Add(new Point(pt.X, pt.Y));

                                    this.QuickExtractingCtrl.btnNewPath.Enabled = false;
                                    this.backgroundWorker1.RunWorkerAsync(new object[] { 1, pt, @params });
                                }
                            }
                            else
                            {
                                this.QuickExtractingCtrl.Alg?.SeedPoints?.Add(new Point(pt.X, pt.Y));
                                this.firstClick = false;
                                this.Cursor = Cursors.Default;
                                _dontDoMouseMove = false;
                                if (this.timer3.Enabled)
                                    this.timer3.Stop();
                                this.timer3.Start();
                            }

                            break;
                        }

                    case 2:
                        {
                            if (this.QuickExtractingCtrl.Alg != null & this.backgroundWorker1.IsBusy == false)
                            {
                                this.QuickExtractingCtrl.Alg?.PrepareForTranslatedPaths(@params.imgDataPic, @params.bmpDataForValueComputation, @params.amountX, @params.amountY);
                                this.QuickExtractingCtrl.Alg?.SetColorBmp(true);
                            }
                            this.Cursor = Cursors.Default;
                            _dontDoMouseMove = false;
                            if (this.timer3.Enabled)
                                this.timer3.Stop();
                            this.timer3.Start();
                            break;
                        }

                    case 3:
                        {
                            if (this.QuickExtractingCtrl.Alg != null && QuickExtractingCtrl.Alg.SeedPoints != null && this.QuickExtractingCtrl.Alg.picAlg?.CostMaps != null && this.ImgDataPic != null)
                            {
                                this.QuickExtractingCtrl.Alg.MouseClicked = false;
                                this.QuickExtractingCtrl.Alg.bgw = this.backgroundWorker1;
                                this.QuickExtractingCtrl.Alg.SeedPoints = @params.SeedPoints;

                                if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0)
                                    this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                                this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint = new Point(System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints?[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints?[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].Y));

                                if (this.QuickExtractingCtrl.Alg.SortedPixelList?.Count == 0)
                                {
                                    Stack<int> addresses = new Stack<int>();
                                    addresses.Push(pt.Y * this.ImgDataPic.Width * 4 + pt.X * 4);
                                    QuickExtractingCtrl.Alg.SortedPixelList.Add(new QEData(0, addresses));
                                }

                                if (!this.backgroundWorker1.IsBusy)
                                {
                                    this.QuickExtractingCtrl.Alg.NotifyEach = @params.notifyEach;
                                    this.QuickExtractingCtrl.btnNewPath.Enabled = false;
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
                            if (this.QuickExtractingCtrl.Alg?.CurPath != null)
                            {
                                if (this.QuickExtractingCtrl.Alg != null && this.QuickExtractingCtrl.Alg.CurPath.Count > 0 && this.QuickExtractingCtrl.Alg.bmp != null && this.QuickExtractingCtrl.Alg.picAlg != null)
                                {
                                    List<PointF> l = this.QuickExtractingCtrl.Alg.CurPath[this.QuickExtractingCtrl.Alg.CurPath.Count - 1]; // todo: take the list with maximum edgeWeight

                                    this.QuickExtractingCtrl.Alg.picAlg.Mcc = new MagnitudeAndPixelCostCalculatorHistoData();
                                    this.QuickExtractingCtrl.Alg.picAlg.Mcc.AddressesGray = l.ConvertAll(a => System.Convert.ToInt32(a.X) + System.Convert.ToInt32(a.Y) * this.helplineRulerCtrl1.Bmp.Width);
                                    this.QuickExtractingCtrl.Alg.picAlg.Mcc.BmpDataForValueComputation = QuickExtractingCtrl.Alg.bmpDataForValueComputation;
                                    this.QuickExtractingCtrl.Alg.picAlg.Mcc.DataGray = QuickExtractingCtrl.Alg.picAlg.GrayRepresentation;
                                    this.QuickExtractingCtrl.Alg.picAlg.Mcc.Dist = @params.dist;
                                    this.QuickExtractingCtrl.Alg.picAlg.Mcc.Stride = this.QuickExtractingCtrl.Alg.bmp.Width * 4;

                                    MagnitudeAndPixelCostMap? mcm = QuickExtractingCtrl.Alg?.picAlg.Mcc.CalculateRamps();

                                    if (this.QuickExtractingCtrl.Alg != null)
                                        this.QuickExtractingCtrl.Alg.picAlg.CostMaps = mcm;

                                    @params.Ramps = this.QuickExtractingCtrl.Alg?.picAlg.CostMaps?.Ramps;
                                    this.QuickExtractingCtrl.Ramps = @params.Ramps;

                                    this.Cursor = Cursors.Default;
                                    _dontDoMouseMove = false;

                                    this.QuickExtractingCtrl.lblTrainInfo.Text = "trained";

                                    if (this.timer3.Enabled)
                                        this.timer3.Stop();
                                    this.timer3.Start();
                                }
                            }
                            break;
                        }

                    case 5:
                        {
                            if (this.QuickExtractingCtrl.Alg != null && this.QuickExtractingCtrl.Alg.picAlg != null)
                            {
                                this.QuickExtractingCtrl.Alg.picAlg.CostMaps = null;

                                @params.Ramps = null;
                                this.QuickExtractingCtrl.Ramps = null;
                            }

                            this.Cursor = Cursors.Default;
                            _dontDoMouseMove = false;

                            this.QuickExtractingCtrl.lblTrainInfo.Text = "not trained";

                            if (this.timer3.Enabled)
                                this.timer3.Stop();
                            this.timer3.Start();
                            break;
                        }

                    case 6:
                        {
                            if ((this.QuickExtractingCtrl.Alg == null || this.firstClick))
                            {
                                this.QuickExtractingCtrl.Alg = new QuickExtractingAlg(@params.imgDataPic, @params.bmpDataForValueComputation, pt, @params.doR, @params.doG, @params.doB, @params.doScale, @params.neighbors, this.backgroundWorker1);
                                this.QuickExtractingCtrl.Alg.SetColorBmp(true);
                                //this.QuickExtractingCtrl.Alg.NotifyEdges += Alg_NotifyEdges;
                            }

                            if (!QuickExtractingCtrl.Alg.AllScanned || (QuickExtractingCtrl.Alg.AllScanned && @params.MouseClicked))
                            {
                                QuickExtractingCtrl.Alg.AllScanned = false;

                                if (this.QuickExtractingCtrl.Alg.picAlg != null && this.QuickExtractingCtrl.Alg.picAlg.CostMaps == null && @params.useCostMap)
                                    this.QuickExtractingCtrl.Alg.InitCostMapStandardCalc();
                                else if (this.QuickExtractingCtrl.Alg.picAlg != null && (!@params.useCostMap))
                                    this.QuickExtractingCtrl.Alg.picAlg.CostMaps = null;

                                if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0 && this.QuickExtractingCtrl.Alg.picAlg?.CostMaps != null)
                                    this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                                this.QuickExtractingCtrl.Alg.valL = @params.valL;
                                this.QuickExtractingCtrl.Alg.valM = @params.valM;
                                this.QuickExtractingCtrl.Alg.valG = @params.valG;
                                this.QuickExtractingCtrl.Alg.laplTh = @params.laplTh;
                                this.QuickExtractingCtrl.Alg.edgeWeight = @params.edgeWeight;
                                this.QuickExtractingCtrl.Alg.dist = @params.dist;

                                this.QuickExtractingCtrl.Alg.CurPath = @params.CurPath;
                                this.QuickExtractingCtrl.Alg.SeedPoints = @params.SeedPoints;

                                this.QuickExtractingCtrl.Alg.valP = @params.valP;
                                this.QuickExtractingCtrl.Alg.valI = @params.valI;
                                this.QuickExtractingCtrl.Alg.valO = @params.valO;

                                this.QuickExtractingCtrl.Alg.valCl = @params.valCl;
                                this.QuickExtractingCtrl.Alg.valCol = @params.valCol;

                                this.QuickExtractingCtrl.Alg.NotifyEach = @params.notifyEach;

                                if (!this.firstClick)
                                {
                                    if (!this.backgroundWorker1.IsBusy)
                                    {
                                        this.QuickExtractingCtrl.Alg.MouseClicked = @params.MouseClicked;
                                        this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint = new Point(pt.X, pt.Y);
                                        this.QuickExtractingCtrl.Alg.ReInit(System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints?[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints?[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].Y), @params.doScale, @params.neighbors);

                                        QuickExtractingCtrl.Alg.CancelFlag = false;
                                        this.QuickExtractingCtrl.btnNewPath.Enabled = false;
                                        this.backgroundWorker1.RunWorkerAsync(new object[] { 6, pt, @params });
                                    }
                                }
                                else
                                {
                                    QuickExtractingCtrl.Alg.SeedPoints?.Add(new Point(pt.X, pt.Y));
                                    this.firstClick = false;
                                    this.Cursor = Cursors.Default;
                                    _dontDoMouseMove = false;

                                    if (this.timer3.Enabled)
                                        this.timer3.Stop();
                                    this.timer3.Start();
                                }
                            }

                            break;
                        }

                    case 7:
                        {
                            if (this.QuickExtractingCtrl.Alg != null && this.QuickExtractingCtrl.Alg.bmp != null)
                            {
                                this.QuickExtractingCtrl.Alg.MouseClicked = false;
                                QuickExtractingCtrl.Alg.bgw = this.backgroundWorker1; // 1234567

                                if (QuickExtractingCtrl.Alg.AllScanned && this.QuickExtractingCtrl.Alg.picAlg?.CostMaps != null && this.QuickExtractingCtrl.Alg.BackPointers != null)
                                {
                                    this.QuickExtractingCtrl.Alg.CancelFlag = true;

                                    if (this.QuickExtractingCtrl.Alg.picAlg.CostMaps == null && @params.useCostMap)
                                        this.QuickExtractingCtrl.Alg.InitCostMapStandardCalc();
                                    else if ((!@params.useCostMap))
                                        this.QuickExtractingCtrl.Alg.picAlg.CostMaps = null;

                                    if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0 && this.QuickExtractingCtrl.Alg.picAlg.CostMaps != null)
                                        this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                                    this.QuickExtractingCtrl.Alg.SeedPoints = @params.SeedPoints;
                                    this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint = new Point(pt.X, pt.Y);

                                    int aaa = this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint.X + this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint.Y * this.QuickExtractingCtrl.Alg.bmp.Width;
                                    int zzz = this.QuickExtractingCtrl.Alg.BackPointers[aaa];

                                    if (zzz != -1)
                                    {
                                        List<PointF>? Path = this.QuickExtractingCtrl.Alg.GetPath(this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint.X, this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint.Y);
                                        Path?.Reverse();

                                        if (Path != null)
                                        {
                                            this.QuickExtractingCtrl.Alg.TempPath = Path;

                                            if (!this.backgroundWorker1.IsBusy)
                                            {
                                                this.QuickExtractingCtrl.Alg.NotifyEach = @params.notifyEach;
                                                this.QuickExtractingCtrl.btnNewPath.Enabled = false;
                                                this.backgroundWorker1.RunWorkerAsync(new object[] { 7, pt, @params });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    this.QuickExtractingCtrl.Alg.CancelFlag = true;

                                    if (this.QuickExtractingCtrl.Alg.picAlg?.CostMaps == null && @params.useCostMap)
                                        this.QuickExtractingCtrl.Alg.InitCostMapStandardCalc();
                                    else if (this.QuickExtractingCtrl.Alg.picAlg != null && (!@params.useCostMap))
                                        this.QuickExtractingCtrl.Alg.picAlg.CostMaps = null;

                                    if (@params.useCostMap && @params.Ramps != null && @params.Ramps.Count > 0 && this.QuickExtractingCtrl.Alg.picAlg?.CostMaps != null)
                                        this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps = @params.Ramps;

                                    this.QuickExtractingCtrl.Alg.CurPath = @params.CurPath;
                                    this.QuickExtractingCtrl.Alg.SeedPoints = @params.SeedPoints;
                                    this.QuickExtractingCtrl.Alg.PreviouslyInsertedPoint = new Point(pt.X, pt.Y);

                                    if (this.QuickExtractingCtrl.Alg.SortedPixelList?.Count == 0 && this.ImgDataPic != null)
                                    {
                                        Stack<int> addresses = new Stack<int>();
                                        addresses.Push(pt.Y * this.ImgDataPic.Width * 4 + pt.X * 4);
                                        QuickExtractingCtrl.Alg.SortedPixelList.Add(new QEData(0, addresses));
                                    }

                                    if (!this.backgroundWorker1.IsBusy)
                                    {
                                        this.QuickExtractingCtrl.Alg.ReInit(System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints?[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].X), System.Convert.ToInt32(QuickExtractingCtrl.Alg.SeedPoints?[QuickExtractingCtrl.Alg.SeedPoints.Count - 1].Y), @params.doScale, @params.neighbors);
                                        this.QuickExtractingCtrl.Alg.NotifyEach = @params.notifyEach;
                                        QuickExtractingCtrl.Alg.CancelFlag = false;
                                        this.QuickExtractingCtrl.btnNewPath.Enabled = false;
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
                                this.QuickExtractingCtrl.AddLine(@params.CurPath, @params.SeedPoints);

                                this.QuickExtractingCtrl.SeedPoints = @params.SeedPoints;
                                this.QuickExtractingCtrl.CurPath = @params.CurPath;
                            }

                            this.firstClick = false;
                            this.Cursor = Cursors.Default;
                            _dontDoMouseMove = false;

                            if (this.timer3.Enabled)
                                this.timer3.Stop();
                            this.timer3.Start();

                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                            break;
                        }

                    case 102:
                        {
                            if (@params.SeedPoints != null && @params.SeedPoints.Count > 0)
                            {
                                @params.TempPath = new List<PointF>();
                                this.QuickExtractingCtrl.TempPath = this.QuickExtractingCtrl.AddLine(@params.TempPath, pt, @params.SeedPoints[@params.SeedPoints.Count - 1]);

                                if (this.QuickExtractingCtrl.cbAutoSeeds.Enabled && this.QuickExtractingCtrl.cbAutoSeeds.Checked && this.QuickExtractingCtrl.TempPath?.Count > 0 && this._tempLast.X != this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1].X &&
                                    this._tempLast.Y != this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1].Y)
                                {
                                    this._tempLast = this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1];
                                    if (this.timer2.Enabled)
                                        this.timer2.Stop();
                                    this.timer2.Start();
                                }
                            }

                            this.firstClick = false;
                            this.Cursor = Cursors.Default;
                            _dontDoMouseMove = false;

                            if (this.timer3.Enabled)
                                this.timer3.Stop();
                            this.timer3.Start();

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
        }

        private void CheckSeedPoints(Bitmap? imgDataPic, List<PointF>? seedPoints)
        {
            if (imgDataPic != null && seedPoints != null)
            {
                int w = imgDataPic.Width;
                int h = imgDataPic.Height;

                for (int j = 0; j < seedPoints.Count; j++)
                {
                    if (seedPoints[j].X < 0)
                        seedPoints[j] = new PointF(0, seedPoints[j].Y);
                    if (seedPoints[j].X > w - 1)
                        seedPoints[j] = new PointF(w - 1, seedPoints[j].Y);
                    if (seedPoints[j].Y < 0)
                        seedPoints[j] = new PointF(seedPoints[j].X, 0);
                    if (seedPoints[j].Y > h - 1)
                        seedPoints[j] = new PointF(seedPoints[j].X, h - 1);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.timer2.Stop();
            if (!this._pathClosed)
                CheckAutoSeedPoint();
        }

        private void CheckAutoSeedPoint()
        {
            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.TempPath != null && this.QuickExtractingCtrl.TempPath.Count > 0 && this._tempLast.X == this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1].X && this._tempLast.Y == this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1].Y)
            {
                if (this.helplineRulerCtrl1.Bmp != null)
                {
                    this.QuickExtractingCtrl.AddTempPath(true, true, this._ix, this._iy);
                    if (this.QuickExtractingCtrl.TempPath != null)
                        this.QuickExtractingCtrl.TempPath.Clear();
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    apply();
                }
            }
        }

        private void cleanCP()
        {
            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.CurPath != null && this.QuickExtractingCtrl.CurPath.Count > 0)
            {
                for (int i = this.QuickExtractingCtrl.CurPath.Count - 1; i >= 0; i += -1)
                {
                    if (this.QuickExtractingCtrl.CurPath[i].Count == 0)
                        this.QuickExtractingCtrl.CurPath.RemoveAt(i);
                }

                // refactor into method
                if (this.QuickExtractingCtrl.CurPath.Count > 0)
                {
                    this.QuickExtractingCtrl.SeedPoints = new List<PointF>();

                    if (this.QuickExtractingCtrl.CurPath[0].Count > 0)
                    {
                        PointF pt = this.QuickExtractingCtrl.CurPath[0][0];
                        this.QuickExtractingCtrl.SeedPoints.Add(new PointF(pt.X, pt.Y));
                    }

                    for (int j = 0; j < this.QuickExtractingCtrl.CurPath.Count; j++)
                    {
                        if (this.QuickExtractingCtrl.CurPath[j].Count > 0)
                        {
                            PointF pt = this.QuickExtractingCtrl.CurPath[j][this.QuickExtractingCtrl.CurPath[j].Count - 1];
                            this.QuickExtractingCtrl.SeedPoints.Add(new PointF(pt.X, pt.Y));
                        }
                        else
                            MessageBox.Show("ffff");
                    }
                }
            }
        }

        private void apply()
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.QuickExtractingCtrl != null &&
                this.SeedPoints != null && this.SeedPoints.Count > this._curPos &&
                    this.BmpForValueComputation != null && this.ImgDataPic != null)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                int ix = (int)this.SeedPoints[this._curPos].X;
                int iy = (int)this.SeedPoints[this._curPos].Y;

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = this.QuickExtractingCtrl.cmbAlgMode.SelectedIndex == 0 ? "run" : "run2",
                    bmpDataForValueComputation = this.BmpForValueComputation,
                    imgDataPic = this.ImgDataPic,
                    MouseClicked = true,
                    Ramps = this.QuickExtractingCtrl.Ramps,
                    SeedPoints = this.QuickExtractingCtrl.SeedPoints,
                    TempPath = this.QuickExtractingCtrl.TempPath,
                    CurPath = this.QuickExtractingCtrl.CurPath,
                    valL = System.Convert.ToDouble(this.numValL.Value),
                    valM = System.Convert.ToDouble(this.numValM.Value),
                    valG = System.Convert.ToDouble(this.numValG.Value),
                    valP = System.Convert.ToDouble(this.numValP.Value),
                    valI = System.Convert.ToDouble(this.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.numValO.Value),
                    valCl = System.Convert.ToDouble(this.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.numEdgeWeight.Value),
                    doScale = this.cbScaleValues.Checked,
                    doR = this.QuickExtractingCtrl.cbR.Checked,
                    doG = this.QuickExtractingCtrl.cbG.Checked,
                    doB = this.QuickExtractingCtrl.cbB.Checked,
                    neighbors = this.QuickExtractingCtrl.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.QuickExtractingCtrl.numTrainDist.Value),
                    useCostMap = this.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.QuickExtractingCtrl.numDisplayEdgeAt.Value)
                };

                if (this.cbAddLine.Checked)
                    this.PrepareBGW(new object[] { 101, new Point(ix, iy), @params });
                else if (this.QuickExtractingCtrl.cmbAlgMode.SelectedIndex == 0)
                    this.PrepareBGW(new object[] { 1, new Point(ix, iy), @params });
                else
                    this.PrepareBGW(new object[] { 6, new Point(ix, iy), @params });
            }
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null && this.QuickExtractingCtrl != null)
            {
                object[] o = (object[])e.Argument;

                int num = System.Convert.ToInt32(o[0]);
                Point pt = (Point)o[1];
                QuickExtractingParameters @params = (QuickExtractingParameters)o[2];

                switch (num)
                {
                    case 1:
                        {
                            int j = 0;
                            if (@params.CurPath != null)
                                j = @params.CurPath.Count;

                            int jj = j + 1;

                            if (this._finished && this.SeedPoints?.Count > 0)
                                jj = 0;

                            this.QuickExtractingCtrl.Alg?.EnumeratePaths(true);

                            @params.CurPath = this.QuickExtractingCtrl.Alg?.CurPath;
                            @params.SeedPoints = this.QuickExtractingCtrl.Alg?.SeedPoints;
                            @params.TempPath = this.QuickExtractingCtrl.Alg?.TempPath;
                            if (this.QuickExtractingCtrl.Alg?.picAlg?.CostMaps != null)
                                @params.Ramps = this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps;

                            if (this.cbAlwaysToSeedPoint.Checked)
                            {
                                bool a = (this.SeedPoints != null && @params.SeedPoints != null &&
                                    this.SeedPoints.Count > jj && @params.SeedPoints.Count > j + 1 &&
                                    (this.SeedPoints[jj].X != @params.SeedPoints[j + 1].X ||
                                    this.SeedPoints[jj].Y != @params.SeedPoints[j + 1].Y));

                                if (this.QuickExtractingCtrl.Alg?.CurPath != null &&
                                    (this.QuickExtractingCtrl.Alg?.CurPath.Count == j || a) &&
                                    @params.SeedPoints != null && @params.SeedPoints.Count > 1)
                                {
                                    if (this.QuickExtractingCtrl.Alg?.CurPath.Count == 0)
                                        @params.CurPath = new List<List<PointF>>();

                                    if (@params.CurPath != null && @params.CurPath.Count != @params.SeedPoints.Count - 1)
                                    {
                                        if (@params.CurPath.Count >= @params.SeedPoints.Count)
                                        {
                                            int i = @params.SeedPoints.Count;
                                            for (int z = i; z < @params.CurPath.Count; z++)
                                            {
                                                if (@params.CurPath[z] != null && @params.CurPath[z].Count > 0)
                                                    @params.SeedPoints.Add(@params.CurPath[z][@params.CurPath[z].Count - 1]);
                                            }
                                        }
                                        else
                                        {
                                            int i = @params.CurPath.Count;
                                            for (int z = i; z < @params.SeedPoints.Count - 1; z++)
                                            {
                                                @params.CurPath.Add(new List<PointF>());
                                                @params.CurPath[i].Add(@params.SeedPoints[z]);
                                                //if (z < @params.SeedPoints.Count - 1)
                                                @params.CurPath[i].Add(@params.SeedPoints[z + 1]);
                                            }
                                        }
                                    }

                                    if ((this.SeedPoints?.Count > jj && @params.SeedPoints.Count > j + 1 &&
                                        this.SeedPoints[jj].X == @params.SeedPoints[j + 1].X &&
                                        this.SeedPoints[jj].Y == @params.SeedPoints[j + 1].Y))
                                    {
                                        if (@params.CurPath != null)
                                            @params.CurPath.Add(new List<PointF>());
                                        if (@params.CurPath != null && @params.CurPath.Count > j)
                                        {
                                            @params.CurPath[j].Add(@params.SeedPoints[@params.SeedPoints.Count - 2]);
                                            @params.CurPath[j].Add(@params.SeedPoints[@params.SeedPoints.Count - 1]);
                                        }
                                    }
                                    else if (this.SeedPoints != null && this.SeedPoints.Count > jj && @params.CurPath != null)
                                    {
                                        //letzter point im path = seedpoint?
                                        if (@params.CurPath[j][@params.CurPath[j].Count - 1] != this.SeedPoints[j])
                                        {
                                            @params.CurPath[j].Add(this.SeedPoints[jj]);
                                            @params.SeedPoints[j + 1] = this.SeedPoints[jj];
                                        }
                                    }
                                }
                            }

                            e.Result = @params;
                            break;
                        }

                    case 2:
                        {
                            if (this.QuickExtractingCtrl.Alg != null)
                                this.QuickExtractingCtrl.Alg.PrepareForTranslatedPaths(@params.imgDataPic, @params.bmpDataForValueComputation, @params.transX, @params.transY);
                            break;
                        }

                    case 3:
                        {
                            int j = 0;
                            if (@params.CurPath != null)
                                j = @params.CurPath.Count;

                            int jj = j + 1;

                            if (this._finished && this.SeedPoints?.Count > 0)
                                jj = 0;

                            if (this.QuickExtractingCtrl.Alg != null)
                            {
                                this.QuickExtractingCtrl.Alg.MouseClicked = false;

                                if (!this.QuickExtractingCtrl.Alg.isRunning)
                                {
                                    this.QuickExtractingCtrl.Alg.ReInit(pt.X, pt.Y, @params.doScale, @params.neighbors);
                                    this.QuickExtractingCtrl.Alg.EnumeratePaths(true);

                                    @params.CurPath = this.QuickExtractingCtrl.Alg.CurPath;
                                    @params.SeedPoints = this.QuickExtractingCtrl.Alg.SeedPoints;
                                    @params.TempPath = this.QuickExtractingCtrl.Alg.TempPath;
                                    if (this.QuickExtractingCtrl.Alg.picAlg?.CostMaps != null)
                                        @params.Ramps = this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps;

                                    if (this.cbAlwaysToSeedPoint.Checked)
                                    {
                                        bool a = (this.SeedPoints != null && @params.SeedPoints != null &&
                                            this.SeedPoints.Count > jj && @params.SeedPoints.Count > j + 1 &&
                                            (this.SeedPoints[jj].X != @params.SeedPoints[j + 1].X ||
                                            this.SeedPoints[jj].Y != @params.SeedPoints[j + 1].Y));

                                        if (this.QuickExtractingCtrl.Alg?.CurPath != null &&
                                            (this.QuickExtractingCtrl.Alg?.CurPath.Count == j || a) &&
                                            @params.SeedPoints != null && @params.SeedPoints.Count > 1)
                                        {
                                            if (this.QuickExtractingCtrl.Alg?.CurPath.Count == 0)
                                                @params.CurPath = new List<List<PointF>>();

                                            if (@params.CurPath != null && @params.CurPath.Count != @params.SeedPoints.Count - 1)
                                            {
                                                if (@params.CurPath.Count >= @params.SeedPoints.Count)
                                                {
                                                    int i = @params.SeedPoints.Count;
                                                    for (int z = i; z < @params.CurPath.Count; z++)
                                                    {
                                                        if (@params.CurPath[z] != null && @params.CurPath[z].Count > 0)
                                                            @params.SeedPoints.Add(@params.CurPath[z][@params.CurPath[z].Count - 1]);
                                                    }
                                                }
                                                else
                                                {
                                                    int i = @params.CurPath.Count;
                                                    for (int z = i; z < @params.SeedPoints.Count - 1; z++)
                                                    {
                                                        @params.CurPath.Add(new List<PointF>());
                                                        @params.CurPath[i].Add(@params.SeedPoints[z]);
                                                        //if (z < @params.SeedPoints.Count - 1)
                                                        @params.CurPath[i].Add(@params.SeedPoints[z + 1]);
                                                    }
                                                }
                                            }

                                            if ((this.SeedPoints?.Count > jj && @params.SeedPoints.Count > j + 1 &&
                                                this.SeedPoints[jj].X == @params.SeedPoints[j + 1].X &&
                                                this.SeedPoints[jj].Y == @params.SeedPoints[j + 1].Y))
                                            {
                                                if (@params.CurPath != null)
                                                    @params.CurPath.Add(new List<PointF>());
                                                if (@params.CurPath != null && @params.CurPath.Count > j)
                                                {
                                                    @params.CurPath[j].Add(@params.SeedPoints[@params.SeedPoints.Count - 2]);
                                                    @params.CurPath[j].Add(@params.SeedPoints[@params.SeedPoints.Count - 1]);
                                                }
                                            }
                                            else if (this.SeedPoints != null && this.SeedPoints.Count > jj && @params.CurPath != null)
                                            {
                                                //letzter point im path = seedpoint?
                                                if (@params.CurPath[j][@params.CurPath[j].Count - 1] != this.SeedPoints[j])
                                                {
                                                    @params.CurPath[j].Add(this.SeedPoints[jj]);
                                                    @params.SeedPoints[j + 1] = this.SeedPoints[jj];
                                                }
                                            }
                                        }
                                    }

                                    e.Result = @params;
                                }
                            }
                            break;
                        }

                    case 6:
                        {
                            this.QuickExtractingCtrl.Alg?.EnumeratePaths(true, @params.msg);

                            @params.CurPath = this.QuickExtractingCtrl.Alg?.CurPath;
                            @params.SeedPoints = this.QuickExtractingCtrl.Alg?.SeedPoints;
                            @params.TempPath = this.QuickExtractingCtrl.Alg?.TempPath;
                            if (this.QuickExtractingCtrl.Alg?.picAlg?.CostMaps != null)
                                @params.Ramps = this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps;

                            e.Result = @params;
                            break;
                        }

                    case 7:
                        {
                            @params.CurPath = this.QuickExtractingCtrl.Alg?.CurPath;
                            @params.SeedPoints = this.QuickExtractingCtrl.Alg?.SeedPoints;
                            @params.TempPath = this.QuickExtractingCtrl.Alg?.TempPath;
                            if (this.QuickExtractingCtrl.Alg?.picAlg?.CostMaps != null)
                                @params.Ramps = this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps;

                            e.Result = @params;
                            break;
                        }

                    case 71:
                        {
                            if (this.QuickExtractingCtrl.Alg != null)
                            {
                                this.QuickExtractingCtrl.Alg.EnumeratePaths(true, @params.msg);

                                @params.CurPath = this.QuickExtractingCtrl.Alg.CurPath;
                                @params.SeedPoints = this.QuickExtractingCtrl.Alg.SeedPoints;
                                @params.TempPath = this.QuickExtractingCtrl.Alg.TempPath;
                                if (this.QuickExtractingCtrl.Alg.picAlg?.CostMaps != null)
                                    @params.Ramps = this.QuickExtractingCtrl.Alg.picAlg.CostMaps.Ramps;

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
            if (!e.Cancelled && e.Result != null && this.QuickExtractingCtrl != null)
            {
                QuickExtractingParameters qe = (QuickExtractingParameters)e.Result;

                bool reRunLine = false;

                //check, if no new points have been found
                if (qe != null && !this.QuickExtractingCtrl.cbRunOnMouseMove.Checked && this.QuickExtractingCtrl.cbAutoAddLine.Checked)
                {
                    int? l = qe.CurPath?.Count - 1;

                    if (qe.CurPath != null && qe.CurPath[qe.CurPath.Count - 1].Count > 1 && qe.SeedPoints != null && qe.SeedPoints.Count > 2)
                        if (qe.CurPath[qe.CurPath.Count - 1][qe.CurPath[qe.CurPath.Count - 1].Count - 2] == qe.SeedPoints[qe.SeedPoints.Count - 2])
                            qe.CurPath[qe.CurPath.Count - 1].RemoveAt(qe.CurPath[qe.CurPath.Count - 1].Count - 2);

                    if (l.HasValue && l.Value > 0)
                    {
                        int? c = qe.CurPath?[l.Value].Count - 1;
                        int? c2 = qe.CurPath?[l.Value - 1].Count - 1;

                        if (c.HasValue && c2.HasValue && c.Value > -1 && c2.Value > -1)
                        {
                            PointF? pt = qe.CurPath?[l.Value][c.Value];

                            if (pt.HasValue)
                            {
                                if (qe.CurPath?.Count - this.QuickExtractingCtrl.CurPath?.Count == 0)
                                {
                                    PointF? pt2 = qe.CurPath?[l.Value - 1][c2.Value];

                                    if (pt2.HasValue && pt2.Value.X == pt.Value.X && pt2.Value.Y == pt.Value.Y)
                                        reRunLine = true;

                                    //test, if prev was line
                                    PointF? pt3 = qe.CurPath?[l.Value][0];

                                    if (qe.CurPath?[l.Value].Count == 2 && this._reRunLineRunPrev &&
                                        pt2.HasValue && pt3.HasValue && pt3.Value.X == pt2.Value.X && pt3.Value.Y == pt2.Value.Y)
                                        reRunLine = true;
                                }
                            }
                        }
                    }

                    this.QuickExtractingCtrl.Ramps = qe.Ramps;
                    this.QuickExtractingCtrl.SeedPoints = qe.SeedPoints;
                    this.QuickExtractingCtrl.TempPath = qe.TempPath;
                    this.QuickExtractingCtrl.CurPath = qe.CurPath;
                }

                if (this.QuickExtractingCtrl.TempPath != null && this.QuickExtractingCtrl.cbAutoSeeds.Enabled && this.QuickExtractingCtrl.cbAutoSeeds.Checked && this.QuickExtractingCtrl.TempPath.Count > 0 && this._tempLast.X != this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1].X && this._tempLast.Y != this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1].Y)
                {
                    this._tempLast = this.QuickExtractingCtrl.TempPath[this.QuickExtractingCtrl.TempPath.Count - 1];
                    if (this.timer2.Enabled)
                        this.timer2.Stop();
                    this.timer2.Start();
                }

                if (this.QuickExtractingCtrl.cbAutoClose.Checked && checkPath())
                    closePath();

                if (this.firstClick)
                    this.firstClick = false;

                //if (this.tempData != null)
                //    this.tempData.Clear();
                //if (this.tempDataZ != null)
                //    this.tempDataZ.Clear();
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.Cursor = Cursors.Default;
                _dontDoMouseMove = false;

                this.QuickExtractingCtrl.btnNewPath.Enabled = true;

                //re init the bgw
                this.backgroundWorker1.Dispose();
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorker1.WorkerReportsProgress = true;
                this.backgroundWorker1.WorkerSupportsCancellation = true;
                this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                //this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

                if (reRunLine && this.QuickExtractingCtrl.cbAutoAddLine.Checked)
                {
                    ReRunLine();
                    this.toolStripStatusLabel1.Text = "Line added";
                }
                else
                {
                    this._reRunLineRunPrev = false;
                    this.toolStripStatusLabel1.Text = "";
                }
            }

            if (this._finished)
            {
                if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                    this.QuickExtractingCtrl?.Alg.UnlockBmpData();

                this._computeFullPath = false;

                if (this.QuickExtractingCtrl?.CurPath != null)
                {
                    List<List<PointF>>? l = CloneList(this.QuickExtractingCtrl.CurPath);
                    if (l != null)
                        this.PathListNew?.Add(l);
                }
            }
            else if (this._computeFullPath && this._curPos <= this.SeedPoints?.Count)
                //if(!this._finished)
                //this.btnComputeStep.PerformClick();
                this.button6_Click(this.btnComputeStep, new EventArgs());

            if (this.timer3.Enabled)
                this.timer3.Stop();
            this.timer3.Start();
        }

        private void ReRunLine()
        {
            //remseg
            this.Button6_Click(this.QuickExtractingCtrl?.btnRemSeg, new EventArgs());

            //addline_lastpoint
            if (this.helplineRulerCtrl1.Bmp != null && this.QuickExtractingCtrl != null &&
                    this.BmpForValueComputation != null && this.ImgDataPic != null)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                int ix = this._ix;
                int iy = this._iy;

                QuickExtractingParameters @params = new QuickExtractingParameters()
                {
                    msg = this.QuickExtractingCtrl.cmbAlgMode.SelectedIndex == 0 ? "run" : "run2",
                    bmpDataForValueComputation = this.BmpForValueComputation,
                    imgDataPic = this.ImgDataPic,
                    MouseClicked = true,
                    Ramps = this.QuickExtractingCtrl.Ramps,
                    SeedPoints = this.QuickExtractingCtrl.SeedPoints,
                    TempPath = this.QuickExtractingCtrl.TempPath,
                    CurPath = this.QuickExtractingCtrl.CurPath,
                    valL = System.Convert.ToDouble(this.numValL.Value),
                    valM = System.Convert.ToDouble(this.numValM.Value),
                    valG = System.Convert.ToDouble(this.numValG.Value),
                    valP = System.Convert.ToDouble(this.numValP.Value),
                    valI = System.Convert.ToDouble(this.numVal_I.Value),
                    valO = System.Convert.ToDouble(this.numValO.Value),
                    valCl = System.Convert.ToDouble(this.numValCl.Value),
                    valCol = System.Convert.ToDouble(this.numValC0l.Value),
                    laplTh = System.Convert.ToInt32(this.numLapTh.Value),
                    edgeWeight = System.Convert.ToDouble(this.numEdgeWeight.Value),
                    doScale = this.cbScaleValues.Checked,
                    doR = this.QuickExtractingCtrl.cbR.Checked,
                    doG = this.QuickExtractingCtrl.cbG.Checked,
                    doB = this.QuickExtractingCtrl.cbB.Checked,
                    neighbors = this.QuickExtractingCtrl.cmbAmntNeighbors.SelectedIndex,
                    dist = System.Convert.ToInt32(this.QuickExtractingCtrl.numTrainDist.Value),
                    useCostMap = this.cbUseCostMaps.Checked,
                    notifyEach = System.Convert.ToInt32(this.QuickExtractingCtrl.numDisplayEdgeAt.Value)
                };

                this._reRunLineRunPrev = true;

                this.PrepareBGW(new object[] { 101, new Point(ix, iy), @params });
            }
        }

        private void closePath()
        {
            if (this.timer1.Enabled)
                this.timer1.Stop();
            if (this.timer2.Enabled)
                this.timer2.Stop();
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();
            if (this.QuickExtractingCtrl != null)
            {
                if (this.QuickExtractingCtrl.CurPath != null && this.QuickExtractingCtrl.CurPath.Count > 0)
                {
                    Nullable<PointF> ff = default(PointF?);
                    List<PointF> f = this.QuickExtractingCtrl.CurPath[0];
                    if (f.Count > 0)
                        ff = f[0];
                    if (ff != null)
                    {
                        List<PointF> l = this.QuickExtractingCtrl.CurPath[this.QuickExtractingCtrl.CurPath.Count - 1];
                        if (l[l.Count - 1].X != ff.Value.X || l[l.Count - 1].Y != ff.Value.Y)
                            l.Add(ff.Value);
                    }
                }

                this.QuickExtractingCtrl.cbRunAlg.Checked = false;
                this.QuickExtractingCtrl.cbRunOnMouseMove.Checked = false;
                this.QuickExtractingCtrl.cbRunAlg.Enabled = false;
                this.QuickExtractingCtrl.cbRunOnMouseMove.Enabled = false;

                this.QuickExtractingCtrl.btnCrop.Enabled = true;
                this.QuickExtractingCtrl.cbCropFromOrig.Enabled = true;
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            this._pathClosed = true;
        }

        private void Button6_Click(object? sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.CurPath != null)
            {
                this.QuickExtractingCtrl.cbRunOnMouseMove.Checked = false;

                this.QuickExtractingCtrl.TempPath = new List<PointF>();

                if (this.QuickExtractingCtrl.CurPath.Count > 0)
                    this.QuickExtractingCtrl.CurPath.RemoveAt(this.QuickExtractingCtrl.CurPath.Count - 1);

                if (this.QuickExtractingCtrl.CurPath.Count == 0)
                    this.QuickExtractingCtrl.CurPath = new List<List<PointF>>();

                if (this.QuickExtractingCtrl.cmbAlgMode.SelectedIndex == 0)
                {
                    if (this.QuickExtractingCtrl.SeedPoints?.Count > 0)
                        this.QuickExtractingCtrl.SeedPoints.RemoveAt(this.QuickExtractingCtrl.SeedPoints.Count - 1);

                    if (this.QuickExtractingCtrl.SeedPoints?.Count == 0)
                    {
                        if (this.QuickExtractingCtrl.Alg != null)
                        {
                            //this.QuickExtractingCtrl.Alg.NotifyEdges -= Alg_NotifyEdges;
                            this.QuickExtractingCtrl.Alg.Dispose();
                            this.QuickExtractingCtrl.Alg = null;

                            this.ImgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            this.BmpForValueComputation = new Bitmap(this.ImgDataPic.Width, this.ImgDataPic.Height);
                        }
                        this.QuickExtractingCtrl.CurPath = new List<List<PointF>>();
                        this.firstClick = true;
                    }
                }
                else
                {
                    if (this.QuickExtractingCtrl.SeedPoints?.Count > 0)
                    {
                        if (this.QuickExtractingCtrl.SeedPoints.Count == 1)
                            _spBU = this.QuickExtractingCtrl.SeedPoints[0];
                        this.QuickExtractingCtrl.SeedPoints.RemoveAt(this.QuickExtractingCtrl.SeedPoints.Count - 1);
                    }

                    if (this.QuickExtractingCtrl.SeedPoints?.Count == 0 && this.QuickExtractingCtrl.CurPath.Count == 0)
                    {
                        if (this.QuickExtractingCtrl.Alg != null)
                        {
                            //this.QuickExtractingCtrl.Alg.NotifyEdges -= Alg_NotifyEdges;
                            this.QuickExtractingCtrl.Alg.Dispose();
                            this.QuickExtractingCtrl.Alg = null;

                            this.ImgDataPic = new Bitmap(this.helplineRulerCtrl1.Bmp);
                            this.BmpForValueComputation = new Bitmap(this.ImgDataPic.Width, this.ImgDataPic.Height);
                        }
                        this.QuickExtractingCtrl.CurPath = new List<List<PointF>>();
                        this.firstClick = true;
                    }
                }

                // refactor into method
                if (this.QuickExtractingCtrl.CurPath.Count > 0)
                {
                    this.QuickExtractingCtrl.SeedPoints = new List<PointF>();

                    if (this.QuickExtractingCtrl.CurPath[0].Count > 0)
                    {
                        PointF pt = this.QuickExtractingCtrl.CurPath[0][0];
                        this.QuickExtractingCtrl.SeedPoints.Add(new PointF(pt.X, pt.Y));
                    }

                    for (int j = 0; j < this.QuickExtractingCtrl.CurPath.Count; j++)
                    {
                        if (this.QuickExtractingCtrl.CurPath[j].Count > 0)
                        {
                            PointF pt = this.QuickExtractingCtrl.CurPath[j][this.QuickExtractingCtrl.CurPath[j].Count - 1];
                            this.QuickExtractingCtrl.SeedPoints.Add(new PointF(pt.X, pt.Y));
                        }
                    }
                }

                if (this.QuickExtractingCtrl.Alg != null)
                    this.QuickExtractingCtrl.Alg.AllScanned = false;

                this.QuickExtractingCtrl.cbRunAlg.Enabled = true;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this._pathClosed = false;
            }
        }

        private bool checkPath()
        {
            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.CurPath != null && this.QuickExtractingCtrl.CurPath.Count > 0 && this.QuickExtractingCtrl.CurPath[0].Count > 0 && this.helplineRulerCtrl1.Bmp != null)
            {
                PointF sp = this.QuickExtractingCtrl.CurPath[0][0];

                int stride = this.helplineRulerCtrl1.Bmp.Width * 4;

                int countF = 0;
                if (this.QuickExtractingCtrl.CurPath.Count > 1)
                {
                    for (int i = 0; i <= this.QuickExtractingCtrl.CurPath.Count - 3; i++)
                        countF += this.QuickExtractingCtrl.CurPath[i].Count;
                }

                for (int l = this.QuickExtractingCtrl.CurPath.Count - 1; l >= 0; l += -1)
                {
                    List<PointF> ListF = this.QuickExtractingCtrl.CurPath[l];
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

                            if (this.QuickExtractingCtrl.CurPath.Count == 1 || l == 0)
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

                        if (l < this.QuickExtractingCtrl.CurPath.Count - 1)
                            this.QuickExtractingCtrl.CurPath.RemoveRange(l + 1, this.QuickExtractingCtrl.CurPath.Count - (l + 1));

                        return true;
                    }
                }
            }

            return false;
        }

        private void applyClose()
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.CurPath != null &&
                    this.QuickExtractingCtrl.CurPath.Count > 0 && this.QuickExtractingCtrl.CurPath[0].Count > 0 &&
                    this.BmpForValueComputation != null && this.ImgDataPic != null)
                {
                    int ix = System.Convert.ToInt32(this.QuickExtractingCtrl.CurPath[0][0].X);
                    int iy = System.Convert.ToInt32(this.QuickExtractingCtrl.CurPath[0][0].Y);

                    QuickExtractingParameters @params = new QuickExtractingParameters()
                    {
                        msg = this.QuickExtractingCtrl.cmbAlgMode.SelectedIndex == 0 ? "run" : "run2",
                        bmpDataForValueComputation = this.BmpForValueComputation,
                        imgDataPic = this.ImgDataPic,
                        MouseClicked = true,
                        Ramps = this.QuickExtractingCtrl.Ramps,
                        SeedPoints = this.QuickExtractingCtrl.SeedPoints,
                        TempPath = this.QuickExtractingCtrl.TempPath,
                        CurPath = this.QuickExtractingCtrl.CurPath,
                        valL = System.Convert.ToDouble(this.numValL.Value),
                        valM = System.Convert.ToDouble(this.numValM.Value),
                        valG = System.Convert.ToDouble(this.numValG.Value),
                        valP = System.Convert.ToDouble(this.numValP.Value),
                        valI = System.Convert.ToDouble(this.numVal_I.Value),
                        valO = System.Convert.ToDouble(this.numValO.Value),
                        valCl = System.Convert.ToDouble(this.numValCl.Value),
                        valCol = System.Convert.ToDouble(this.numValC0l.Value),
                        laplTh = System.Convert.ToInt32(this.numLapTh.Value),
                        edgeWeight = System.Convert.ToDouble(this.numEdgeWeight.Value),
                        doScale = this.cbScaleValues.Checked,
                        doR = this.QuickExtractingCtrl.cbR.Checked,
                        doG = this.QuickExtractingCtrl.cbG.Checked,
                        doB = this.QuickExtractingCtrl.cbB.Checked,
                        neighbors = this.QuickExtractingCtrl.cmbAmntNeighbors.SelectedIndex,
                        dist = System.Convert.ToInt32(this.QuickExtractingCtrl.numTrainDist.Value),
                        useCostMap = this.cbUseCostMaps.Checked,
                        notifyEach = System.Convert.ToInt32(this.QuickExtractingCtrl.numDisplayEdgeAt.Value)
                    };

                    if (this.cbAddLine.Checked)
                        this.PrepareBGW(new object[] { 101, new Point(ix, iy), @params });
                    else if (this.QuickExtractingCtrl.cmbAlgMode.SelectedIndex == 0)
                        this.PrepareBGW(new object[] { 1, new Point(ix, iy), @params });
                    else
                        this.PrepareBGW(new object[] { 6, new Point(ix, iy), @params });
                }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (this.SeedPoints != null && this.ListBox1.SelectedIndex > -1)
            {
                if (this.SeedPoints.Count > this.ListBox1.SelectedIndex)
                {
                    int j = this.ListBox1.SelectedIndex;

                    this.SeedPoints.Insert(this.ListBox1.SelectedIndex + 1, new PointF((float)this.NumericUpDown1.Value, (float)this.NumericUpDown2.Value));
                    this.SeedPointsZ?.Insert(this.ListBox1.SelectedIndex + 1,
                        new PointF((float)this.NumericUpDown1.Value * this.helplineRulerCtrl1.Zoom,
                        (float)this.NumericUpDown2.Value * this.helplineRulerCtrl1.Zoom));
                    this.ListBox1.Items.Insert(this.ListBox1.SelectedIndex + 1, new PointF((float)this.NumericUpDown1.Value, (float)this.NumericUpDown2.Value));

                    this.ListBox1.SelectedIndex = (this.ListBox1.Items.Count > j + 1) ? j + 1 : 0;

                    this._curPos = 0;
                    this.firstClick = true;
                    if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                        this.QuickExtractingCtrl?.Alg.UnlockBmpData();

                    this.helplineRulerCtrl1?.dbPanel1.Invalidate();
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (this.SeedPoints != null && this.ListBox1.SelectedIndex > -1)
            {
                if (this.SeedPoints.Count > this.ListBox1.SelectedIndex)
                {
                    int j = this.ListBox1.SelectedIndex;
                    this.SeedPoints[this.ListBox1.SelectedIndex] = new PointF((float)this.NumericUpDown1.Value, (float)this.NumericUpDown2.Value);
                    if (this.SeedPointsZ != null)
                        this.SeedPointsZ[this.ListBox1.SelectedIndex] = new PointF((float)this.NumericUpDown1.Value * this.helplineRulerCtrl1.Zoom,
                            (float)this.NumericUpDown2.Value * this.helplineRulerCtrl1.Zoom);
                    this.ListBox1.Items[this.ListBox1.SelectedIndex] = new PointF((float)this.NumericUpDown1.Value, (float)this.NumericUpDown2.Value);

                    this.ListBox1.SelectedIndex = (this.ListBox1.Items.Count > j) ? j : 0;

                    this._curPos = 0;
                    this.firstClick = true;
                    if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                        this.QuickExtractingCtrl?.Alg.UnlockBmpData();

                    this.helplineRulerCtrl1?.dbPanel1.Invalidate();
                }
            }
        }

        private void btnRemSgmnt_Click(object sender, EventArgs e)
        {
            if (this.SeedPoints != null && this._curPos > this.SeedPoints.Count + 1)
                this._curPos = this.SeedPoints.Count + 1;

            if (this._curPos > 0)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.QuickExtractingCtrl?.Alg?.bmpDataForValueComputation != null)
                    this.QuickExtractingCtrl?.Alg.UnlockBmpData();

                if (this.QuickExtractingCtrl?.Alg != null && this.QuickExtractingCtrl.SeedPoints != null &&
                    this.QuickExtractingCtrl.CurPath != null && this.QuickExtractingCtrl.CurPath.Count > 0)
                {
                    this._curPos--;
                    this._drawPathPart = true;
                    this.QuickExtractingCtrl.SeedPoints.RemoveAt(this.QuickExtractingCtrl.SeedPoints.Count - 1);
                    this.QuickExtractingCtrl.CurPath.RemoveAt(this.QuickExtractingCtrl.CurPath.Count - 1);
                    this._finished = false;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
                else if (this.QuickExtractingCtrl?.CurPath?.Count == 0)
                {
                    this._curPos--;
                    this._finished = false;
                    this._drawPathPart = true;
                }
            }

            if (this._curPos == 0)
                this.firstClick = true;

            this.label25.Text = "CurPos: " + this._curPos.ToString();
        }

        private void cbAutoAddLine_CheckedChanged(object sender, EventArgs e)
        {
            this.cbAddLine.Enabled = !cbAutoAddLine.Checked;
        }

        private void btnReLoad_Click(object sender, EventArgs e)
        {
            if (this.QuickExtractingCtrl != null && this.QuickExtractingCtrl.PathList != null && this.QuickExtractingCtrl.PathList.Count > 0)
            {
                this.cmbPaths.Items.Clear();
                this.QuickExtractingCtrl.PathList = this._pathListBU;
                if (this.cbLoadAsOne.Checked)
                {
                    List<List<List<PointF>>> bigPath = new();
                    for (int i = 0; i < this.QuickExtractingCtrl.PathList?.Count; i++)
                    {
                        bigPath.Add(new List<List<PointF>>());
                        bigPath[i].Add(new List<PointF>());
                        List<List<PointF>> cur = this.QuickExtractingCtrl.PathList[i];

                        for (int j = 0; j < cur.Count; j++)
                            bigPath[i][0].AddRange(cur[j].ToArray());
                    }

                    this.PathList = CloneList(bigPath);
                    this.PathListNew = new List<List<List<PointF>>>();

                    this.QuickExtractingCtrl.PathList = bigPath;

                    for (int i = 0; i < bigPath.Count; i++)
                        this.cmbPaths.Items.Add("SavedPath_" + i.ToString() + "; " + this.QuickExtractingCtrl.PathList[i][0].Count.ToString());

                    this.cmbPaths.SelectedIndex = 0;
                }
                else
                {
                    for (int i = 0; i < this.QuickExtractingCtrl.PathList?.Count; i++)
                        this.cmbPaths.Items.Add("SavedPath_" + i.ToString() + "; " + this.QuickExtractingCtrl.PathList[i][0].Count.ToString());

                    this.cmbPaths.SelectedIndex = 0;

                    this.PathList = CloneList(this.QuickExtractingCtrl.PathList);
                    this.PathListNew = new List<List<List<PointF>>>();
                }
            }
        }

        private void btnLoadSeedPoints_Click(object sender, EventArgs e)
        {
            this.SeedPoints = new List<PointF>();

            if (this.QuickExtractingCtrl != null)
            {
                if (this.SeedPointsBU != null)
                {
                    this.ListBox1.Items.Clear();
                    this.ListBox1.BeginUpdate();

                    for (int j = 0; j < this.SeedPointsBU.Count; j++)
                    {
                        this.SeedPoints.Add(this.SeedPointsBU[j]);
                        this.ListBox1.Items.Add(this.SeedPointsBU[j]);
                    }

                    this.ListBox1.EndUpdate();

                    this.SeedPointsZ = GetTransformedSP(this.SeedPoints);
                    this._drawPath = this._drawPathPart = false;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.firstClick = true;
                    this._curPos = 0;

                    if (this.QuickExtractingCtrl.Alg != null)
                        this.QuickExtractingCtrl.Alg.UnlockBmpData();
                }
            }
        }

        private void btnBackupSP_Click(object sender, EventArgs e)
        {
            if (this.SeedPoints != null)
            {
                this.BackupSeedPoints(this.SeedPoints);
                this.label23.Enabled = this.btnLoadSeedPoints.Enabled = true;
            }
        }

        private void loadSeedPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._dontDoMouseMove = true;
            this.helplineRulerCtrl1.Enabled = false;
            this.helplineRulerCtrl1.Refresh();

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream? stream = null;
                SavedSeedPoints? svsp = null;

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

                    stream = new FileStream(this.openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    svsp = JsonSerializer.Deserialize<SavedSeedPoints>(stream, options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                }

                try
                {
                    //stream.Close()
                    stream?.Dispose();
                    stream = null;
                }
                catch
                { }

                this.SeedPoints = new List<PointF>();

                if (this.QuickExtractingCtrl != null)
                {
                    if (svsp != null)
                    {
                        this.ListBox1.Items.Clear();
                        this.ListBox1.BeginUpdate();

                        for (int j = 0; j < svsp.SeedPoints?.Length; j++)
                        {
                            this.SeedPoints.Add(svsp.SeedPoints[j]);
                            this.ListBox1.Items.Add(svsp.SeedPoints[j]);
                        }

                        this.ListBox1.EndUpdate();

                        this.SeedPointsZ = GetTransformedSP(this.SeedPoints);
                        this._drawPath = this._drawPathPart = false;

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        this.firstClick = true;
                        this._curPos = 0;

                        if (this.QuickExtractingCtrl.Alg != null)
                            this.QuickExtractingCtrl.Alg.UnlockBmpData();
                    }
                }

                if (this.timer4.Enabled)
                    this.timer4.Stop();
                this.timer4.Start();
            }
        }

        private void saveSeedPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SeedPoints != null && this.SeedPoints.Count > 0 && this.saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                SavedSeedPoints svsp = new();

                PointF[] pts = this.SeedPoints.ToArray();
                svsp.SeedPoints = pts;

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

                    using FileStream createStream = File.Create(this.saveFileDialog2.FileName);
                    JsonSerializer.Serialize(createStream, svsp, options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.timer3.Stop();
            this.label24.Text = "done";
            this.btnComputeStep.Enabled = this.btnComputePath.Enabled = this.helplineRulerCtrl1.Enabled = true;
            this.btnComputeStep.Refresh();
            this.btnComputePath.Refresh();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            this.timer4.Stop();
            this.helplineRulerCtrl1.Enabled = true;
            this._dontDoMouseMove = false;
        }
    }
}
