using ChainCodeFinder;
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

namespace QuickExtract2
{
    public partial class frmPathDetails : Form
    {
        private bool _bloaded = false;
        private GraphicsPath? _fPathComplete;
        private Bitmap? _bmpBU = null;
        private GraphicsPath? _fPath = new GraphicsPath();
        private GraphicsPath? _fPathOrig;
        private List<int>? _newFiguresOrig;
        //private int _bmpWidth;
        private Rectangle _r;
        private int _selectedRow = -1;
        //private Bitmap _img;
        private PointF _curPt;
        private float _offsetX = -1;
        private float _offsetY = -1;
        private bool _batchEdit;
        //private bool _useNormalDist;
        private double[]? _mappings;
        //private bool _even;
        //private bool _zoomWidth;
        private List<int>? _indices;
        private float _zoom = 1.0F;
        private Rectangle? _r2;
        private int _ix;
        private int _iy;
        private PointF _movedPoint = new PointF(-1, -1);
        private List<int>? _newFigures;

        private ShiftPathMode _shiftPathMode = ShiftPathMode.FillPath;
        private ShiftFractionMode _shiftFractionMode = ShiftFractionMode.Closest;
        private GraphicsPath? _shiftedPath;
        private bool _pointsAmountChanged;

        public event EventHandler<FPathEventArgs>? OrigFPathProvide;
        // Private _fT As List(Of Byte)

        public GraphicsPath? FPath
        {
            get
            {
                if (_fPath != null)
                    return (GraphicsPath)_fPath.Clone();
                else
                    return null;
            }
            set
            {
                if (value != null)
                    _fPath = (GraphicsPath)value.Clone();
                else
                    _fPath = null;
            }
        }

        public GraphicsPath? FPathComplete
        {
            get
            {
                if (this._fPathComplete != null && this._fPath != null)
                {
                    if (!this._pointsAmountChanged)
                    {
                        GraphicsPath? fP = null;

                        using (GraphicsPath gP = (GraphicsPath)_fPathComplete.Clone())
                        {
                            if (this._indices != null)
                                fP = InsertPathPart(gP, this._fPath, this._indices, null);
                            if (fP == null)
                                fP = (GraphicsPath)gP.Clone();
                        }

                        using (Matrix mx = new Matrix(1, 0, 0, 1, _r.X, _r.Y))
                        {
                            fP.Transform(mx);
                        }

                        return fP;
                    }
                    else if (MessageBox.Show("Try path-connect?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        GraphicsPath? fP = null;

                        using (GraphicsPath gP = (GraphicsPath)_fPathComplete.Clone())
                        {
                            using (frmInfo frmInfo = new frmInfo())
                            {
                                frmInfo.TopMost = true;
                                frmInfo.StartPosition = FormStartPosition.CenterScreen;
                                frmInfo.Show();
                                if (this._indices != null)
                                    fP = InsertPathPart(gP, this._fPath, this._indices, frmInfo);
                                if (fP == null)
                                    fP = (GraphicsPath)gP.Clone();
                                frmInfo.Close();
                            }
                        }

                        using (Matrix mx = new Matrix(1, 0, 0, 1, _r.X, _r.Y))
                        {
                            fP.Transform(mx);
                        }

                        OrigFPathProvide?.Invoke(this, new FPathEventArgs(this._fPath, _r.X, _r.Y)); // fPath will be disposed when this form closes

                        return fP;
                    }
                    else
                    {
                        GraphicsPath fP = (GraphicsPath)this._fPath.Clone();
                        using (Matrix mx = new Matrix(1, 0, 0, 1, _r.X, _r.Y))
                        {
                            fP.Transform(mx);
                        }

                        return fP;
                    }
                }
                else
                    return null;
            }
        }

        public frmPathDetails(Bitmap Bmp, GraphicsPath FPath, Rectangle r)
        {
            // Dieser Aufruf ist für den Windows Form-Designer erforderlich.
            InitializeComponent();

            if (AvailMem.AvailMem.checkAvailRam(Bmp.Width * Bmp.Height * 8L))
            {
                if (AvailMem.AvailMem.checkAvailRam(Bmp.Width * Bmp.Height * 8L))
                {
                    this.PictureBox1.Image = (Bitmap)Bmp.Clone();
                    _bmpBU = (Bitmap)Bmp.Clone();
                }
                else
                {
                    MessageBox.Show("Not enough Memory");
                    return;
                }

                _r = r;
                _bloaded = true;

                _fPathComplete = (GraphicsPath)FPath.Clone();

                using (Matrix mx = new Matrix(1, 0, 0, 1, -_r.X, -_r.Y))
                {
                    _fPathComplete.Transform(mx);
                }

                if (_fPath != null && this._newFigures != null)
                {
                    _fPath = GetAffectedPathPart(_fPathComplete, new RectangleF(0, 0, _r.Width, _r.Height));
                    this._fPathOrig = (GraphicsPath)this._fPath.Clone();
                    this._newFiguresOrig = new List<int>();
                    this._newFiguresOrig.AddRange(this._newFigures);

                    this.DataGridView1.Rows.Clear();

                    if (_fPath.PointCount > 0)
                    {
                        int i = 0;
                        foreach (PointF pt in _fPath.PathPoints)
                        {
                            i += 1;
                            this.DataGridView1.Rows.Add(i * 100, pt.X, pt.Y);
                        }
                    }
                }

                this.CheckBox1_CheckedChanged(this.CheckBox1, new EventArgs());
            }
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }
        }

        private GraphicsPath? InsertPathPart(GraphicsPath gP, GraphicsPath fPath, List<int> indices, frmInfo? frm)
        {
            if (fPath != null && fPath.PointCount > 1)
            {
                if (this._pointsAmountChanged || (gP.PointCount - indices.Count) + fPath.PointCount > gP.PointCount)
                {
                    List<List<int>> indicesLists = SplitIndices(indices);

                    List<PointF> pList = new List<PointF>();
                    int cnt = 0;
                    int curSum = 0;
                    if (indicesLists.Count > 0)
                    {
                        int fPathStrt = 0;
                        int gPStrt = 0;

                        for (int i = 0; i <= indicesLists.Count - 1; i++)
                        {
                            if (frm != null)
                            {
                                frm.Label1.Text = "Processing list " + (i + 1).ToString() + " of " + indicesLists.Count.ToString();
                                frm.Label1.Refresh();
                            }
                            if (indicesLists[i].Count > 0 && _fPathOrig != null)
                            {
                                for (int j = gPStrt; j <= indicesLists[i][0] - 1; j++)
                                    pList.Add(gP.PathPoints[j]);
                                cnt += indicesLists[i].Count;
                                gPStrt = indicesLists[i][indicesLists[i].Count - 1] + 1;

                                curSum += indicesLists[i].Count;
                                PointF pt = _fPathOrig.PathPoints[curSum - 1];
                                int indx = -1;

                                double minDist = double.MaxValue;

                                for (int j = Math.Max(fPathStrt, 0); j <= fPath.PointCount - 1; j++)
                                {
                                    double dx = pt.X - fPath.PathPoints[j].X;
                                    double dy = pt.Y - fPath.PathPoints[j].Y;

                                    double dist = Math.Sqrt(dx * dx + dy * dy);

                                    if (dist < minDist)
                                    {
                                        indx = j;
                                        minDist = dist;
                                    }
                                }

                                if (pList.Count > 0)
                                {
                                    PointF lPgP = pList[pList.Count - 1];
                                    int indxF = -1;

                                    double mD = double.MaxValue;

                                    for (int j = fPathStrt + 1; j <= indx; j++) // maybe from fPathStrt + 0
                                    {
                                        double dx = lPgP.X - fPath.PathPoints[j].X;
                                        double dy = lPgP.Y - fPath.PathPoints[j].Y;

                                        double dist = Math.Sqrt(dx * dx + dy * dy);

                                        if (dist < mD)
                                        {
                                            indxF = j;
                                            mD = dist;
                                        }
                                    }

                                    fPathStrt = indxF;
                                }

                                for (int j = Math.Max(fPathStrt, 0); j <= indx; j++)
                                    pList.Add(fPath.PathPoints[j]);

                                fPathStrt = indx;
                            }
                        }

                        if (frm != null)
                        {
                            frm.Label1.Text = "Finishing actions ...";
                            frm.Label1.Refresh();
                        }

                        for (int j = gPStrt; j <= gP.PointCount - 1; j++)
                            pList.Add(gP.PathPoints[j]);

                        GraphicsPath gP4 = new GraphicsPath();
                        gP4.AddLines(pList.ToArray());

                        return gP4;
                    }

                    return null;
                }
                else
                {
                    PointF[] pts = new PointF[gP.PointCount - 1 + 1]; // _fPathComplete
                    gP.PathPoints.CopyTo(pts, 0);
                    PointF[] pts2 = new PointF[fPath.PointCount - 1 + 1];
                    fPath.PathPoints.CopyTo(pts2, 0);

                    int l = Math.Min(indices.Count, fPath.PointCount);

                    for (int i = 0; i <= l - 1; i++)
                        pts[indices[i]] = pts2[i];

                    // bei addedPoints gP:pathTypes in Liste kopieren,
                    // Liste "AddedPoints" verabeiten = Points und Index auslesen und in pts und Liste mit Pathtypes insertieren.

                    GraphicsPath fP = new GraphicsPath(pts, gP.PathTypes);

                    return fP;
                }
            }
            else
                return null;
        }

        private List<List<int>> SplitIndices(List<int> indices)
        {
            int i = 0;

            List<List<int>> lOut = new List<List<int>>();

            while (i < indices.Count - 1)
            {
                List<int> l = new List<int>();
                while (i < indices.Count - 1 && Math.Abs(indices[i] - indices[i + 1]) == 1)
                {
                    l.Add(indices[i]);
                    i += 1;
                }
                l.Add(indices[i]);
                i += 1;
                lOut.Add(l);
            }

            return lOut;
        }

        private GraphicsPath GetAffectedPathPart(GraphicsPath fPathComplete, RectangleF r)
        {
            List<PointF> fL = new List<PointF>();
            PointF[] pts = new PointF[fPathComplete.PointCount - 1 + 1];
            fPathComplete.PathPoints.CopyTo(pts, 0);
            List<byte> fT = new List<byte>();
            byte[] t = new byte[fPathComplete.PointCount - 1 + 1];
            fPathComplete.PathTypes.CopyTo(t, 0);

            List<int> indices = new List<int>();
            this._newFigures = new List<int>();

            for (int i = 0; i <= pts.Length - 1; i++)
            {
                if (r.Contains(pts[i]))
                {
                    if (i > 0 && t[i] == 0)
                        this._newFigures.Add(fL.Count);
                    fL.Add(pts[i]);
                    fT.Add(t[i]);
                    indices.Add(i);
                }
            }

            this._newFigures.Add(fL.Count);

            // Me._fT = fT

            if (fL.Count > 1)
            {
                GraphicsPath gP = new GraphicsPath(fL.ToArray(), fT.ToArray());

                this._indices = indices;
                return gP;
            }
            else
                return new GraphicsPath();
        }

        private GraphicsPath GetAffectedPathPart2(GraphicsPath fPathComplete, GraphicsPath fPath, RectangleF r)
        {
            List<PointF> fL = new List<PointF>();
            PointF[] pts = new PointF[fPathComplete.PointCount - 1 + 1];
            fPathComplete.PathPoints.CopyTo(pts, 0);
            fL.AddRange(pts);
            List<byte> fT = new List<byte>();
            byte[] t = new byte[fPathComplete.PointCount - 1 + 1];
            fPathComplete.PathTypes.CopyTo(t, 0);
            fT.AddRange(t);

            for (int i = pts.Length - 1; i >= 0; i += -1)
            {
                if (this._indices != null && this._indices.Contains(i))
                {
                    if (i == 0 && this._indices.Contains(i + 1) && this._indices.Contains(fPathComplete.PointCount - 1))
                    {
                        fL.RemoveAt(i);
                        fT.RemoveAt(i);
                    }
                    if (i > 0 && i < pts.Length - 1 && this._indices.Contains(i + 1) && this._indices.Contains(i - 1))
                    {
                        fL.RemoveAt(i);
                        fT.RemoveAt(i);
                    }
                    if (i == pts.Length - 1 && this._indices.Contains(0) && this._indices.Contains(i - 1))
                    {
                        fL.RemoveAt(i);
                        fT.RemoveAt(i);
                    }
                }
            }

            if (fL.Count > 1)
            {
                GraphicsPath gP = new GraphicsPath(fL.ToArray(), fT.ToArray());
                // gP.AddPath(fPath, False)

                return gP;
            }
            else
                return new GraphicsPath();
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            this._mappings = new double[] { };
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DisplayChanges();
        }

        private void CheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox12.Checked)
                this.SplitContainer2.Panel2.BackColor = SystemColors.ControlDarkDark;
            else
                this.SplitContainer2.Panel2.BackColor = SystemColors.Control;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            DisplayChanges();
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            //if (this._bloaded)
            //{
            //    frmCurveShape frm = new frmCurveShape();
            //    if (frm.ShowDialog() == DialogResult.OK)
            //    {
            //        if (frm.CheckBox1.Checked)
            //        {
            //            if (frm.Points != null && frm.Points.Count > 0)
            //            {
            //                // shift to zero for larger sigmas
            //                double min = frm.Points.Min(a => a.Y);
            //                double max = frm.Points.Max(a => a.Y) - min;

            //                if (max != 0.0)
            //                {
            //                    double factor = 1.0 / max;
            //                    List<double> pts = frm.Points.Select(a => System.Convert.ToDouble((a.Y - min) * factor)).ToList();
            //                    double[] newMappings = new double[pts.Count - 1 + 1];
            //                    pts.CopyTo(newMappings, 0);

            //                    this._useNormalDist = true;
            //                    this._mappings = newMappings;
            //                }
            //            }
            //        }
            //        else if (frm.Mappings != null && frm.Mappings.Count > 0)
            //        {
            //            List<double> mappings = frm.Mappings.Select(a => System.Convert.ToDouble(a / 255.0)).ToList();
            //            double[] newMappings = new double[mappings.Count - 1 + 1];
            //            mappings.CopyTo(newMappings, 0);
            //            bool even = (mappings.Count & 0x1) == 0;

            //            this._useNormalDist = false;
            //            this._mappings = newMappings;
            //            this._even = even;
            //        }
            //    }
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_fPath != null)
            {
                RectangleF r = _fPath.GetBounds();
                SizeF sz = r.Size;

                PointF c = new PointF((float)(r.X + (r.Width / (double)2.0F)), (float)(r.Y + (r.Height / (double)2.0F)));

                using (Matrix m = new Matrix(1, 0, 0, 1, -c.X, -c.Y))
                {
                    _fPath.Transform(m);
                }

                double w = (sz.Width + (System.Convert.ToDouble(numericUpDown1.Value) * 2.0)) / (double)sz.Width;
                double h = (sz.Height + (System.Convert.ToDouble(numericUpDown4.Value) * 2.0)) / (double)sz.Height;
                using (Matrix mx = new Matrix(System.Convert.ToSingle(w), 0, 0, System.Convert.ToSingle(h), 0, 0))
                {
                    _fPath.Transform(mx);
                }

                using (Matrix m = new Matrix(1, 0, 0, 1, c.X, c.Y))
                {
                    _fPath.Transform(m);
                }

                DisplayChanges();

                //_dirty = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.CheckBox5.Checked)
            {
                if (this._bloaded)
                {
                    try
                    {
                        DataGridView dg = this.DataGridView1;

                        DataGridViewSelectedRowCollection sel = dg.SelectedRows;
                        int cnt = sel.Cast<DataGridViewRow>().Count();

                        // compute newMappings by pathLength instead of PointCount
                        double pl1 = 0;
                        double pl2 = 0;
                        int plG = 0;

                        if (_fPath != null && dg.SelectedRows[cnt - 1].Index < dg.SelectedRows[0].Index)
                        {
                            pl1 = CalcPathLength(dg.SelectedRows[cnt - 1].Index, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1));
                        }
                        else if(_fPath != null)
                        {
                            pl1 = CalcPathLength(0, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            pl2 = CalcPathLength(_fPath.PointCount - (_fPath.PointCount - dg.SelectedRows[cnt - 1].Index), dg.Rows.Count - 1, _fPath.PathPoints, true);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1 + pl2));
                        }

                        double[] newMappings = new double[] { };

                        if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                        {
                            //if (this._useNormalDist)
                            //    newMappings = RestrictOrProlongatePoints(this._mappings, plG);
                            //else
                            {
                                newMappings = RestrictOrProlongatePoints(this._mappings, plG / 2 + 2);

                                double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                newMappings.CopyTo(tmp, 0);
                                List<double> lTmp = newMappings.ToList();
                                lTmp.RemoveAt(lTmp.Count - 1);
                                lTmp.Reverse();
                                lTmp.CopyTo(tmp, lTmp.Count + 1);

                                newMappings = tmp;
                            }

                            this._batchEdit = true;
                            double cpl = 0;

                            if (dg.SelectedRows[cnt - 1].Index > dg.SelectedRows[0].Index)
                            {
                                for (int j = 0; j <= dg.SelectedRows[0].Index; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(cpl + pl2), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["x"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["x"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }

                                cpl = 0;

                                for (int j = dg.SelectedRows[cnt - 1].Index; j <= dg.Rows.Count - 1; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && j > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(plG - cpl), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["x"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["x"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }
                            }
                            else
                                for (int i = dg.SelectedRows[cnt - 1].Index; i <= dg.SelectedRows[0].Index; i++)
                                {
                                    if ((dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                    {
                                        double curDist = 0;
                                        if (i > 0 && i > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                        {
                                            double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                            double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                            curDist = Math.Sqrt(dx * dx + dy * dy);
                                            cpl += curDist;
                                        }

                                        int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(pl1 - cpl), newMappings.Count() - 1), 0);

                                        string? s = dg.Rows[i].Cells["x"].Value.ToString();
                                        if (s != null)
                                            dg.Rows[i].Cells["x"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                    }
                                }
                            this._batchEdit = false;
                        }
                        else
                        {
                            this._batchEdit = true;
                            foreach (DataGridViewRow dr in dg.SelectedRows)
                            {

                                if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                                {
                                    string? s = dr.Cells["x"].Value.ToString();
                                    if (s != null)
                                        dr.Cells["x"].Value = float.Parse(s) + System.Convert.ToSingle(this.numericUpDown2.Value);
                                }
                            }
                            this._batchEdit = false;
                        }

                        List<List<PointF>> pts = GetPointsFromDGV(dg);

                        SetupNewPath(pts);
                    }
                    catch
                    {
                    }
                }
            }
            else if (this._bloaded)
            {
                try
                {
                    DataGridView dg = this.DataGridView1;

                    int cnt = this.DataGridView1.Rows.Count;
                    double[] newMappings = new double[] { };

                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0 && _fPath != null)
                    {
                        //if (this._useNormalDist)
                        //    newMappings = RestrictOrProlongatePoints(this._mappings, cnt);
                        //else
                        {
                            newMappings = RestrictOrProlongatePoints(this._mappings, cnt / 2 + 2);

                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                            newMappings.CopyTo(tmp, 0);
                            List<double> lTmp = newMappings.ToList();
                            lTmp.RemoveAt(lTmp.Count - 1);
                            lTmp.Reverse();
                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                            newMappings = tmp;
                        }

                        this._batchEdit = true;
                        int i = 0;

                        double cpl = 0;
                        double pl1 = CalcPathLength(0, dg.Rows.Count - 1, _fPath.PathPoints.ToArray(), false);

                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                double curDist = 0;
                                if (i > 0)
                                {
                                    double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                    double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                    curDist = Math.Sqrt(dx * dx + dy * dy);
                                    cpl += curDist;
                                }

                                double v = cpl / pl1;
                                int mIndx = Math.Min(System.Convert.ToInt32((1.0 - v) * (newMappings.Count() - 1)), newMappings.Count() - 1);

                                string? s = dr.Cells["x"].Value.ToString();
                                if (s != null)
                                    dr.Cells["x"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);

                                i += 1;
                            }
                        }
                        this._batchEdit = false;
                    }
                    else
                    {
                        this._batchEdit = true;
                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                string? s = dr.Cells["x"].Value.ToString();
                                if (s != null)
                                    dr.Cells["x"].Value = float.Parse(s) + System.Convert.ToSingle(this.numericUpDown2.Value);
                            }
                        }
                        this._batchEdit = false;
                    }

                    List<List<PointF>> pts = GetPointsFromDGV(dg);

                    SetupNewPath(pts);
                }
                catch
                {
                }
            }
            if (this.Timer1.Enabled)
                this.Timer1.Stop();
            this.Timer1.Start();
        }

        private double[] RestrictOrProlongatePoints(double[] orig, int pointCount)
        {
            double factor = pointCount / (double)orig.Count();
            double[] pts = new double[pointCount - 1 + 1];

            for (int i = 0; i <= pts.Length - 1; i++)
            {
                double d = i / factor;

                int l = System.Convert.ToInt32(Math.Floor(d));
                int r = System.Convert.ToInt32(Math.Ceiling(d));
                double f = d - l;
                double t = 1 - f;

                if (l > orig.Count() - 1)
                    l -= 1;
                if (r > orig.Count() - 1)
                    r -= 1;

                double x = orig[l] * f + orig[r] * t;

                pts[i] = x;
            }

            return pts;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.CheckBox5.Checked)
            {
                if (this._bloaded)
                {
                    try
                    {
                        DataGridView dg = this.DataGridView1;

                        DataGridViewSelectedRowCollection sel = dg.SelectedRows;
                        int cnt = sel.Cast<DataGridViewRow>().Count();

                        // compute newMappings by pathLength instead of PointCount
                        double pl1 = 0;
                        double pl2 = 0;
                        int plG = 0;

                        if (dg.SelectedRows[cnt - 1].Index < dg.SelectedRows[0].Index && _fPath != null)
                        {
                            pl1 = CalcPathLength(dg.SelectedRows[cnt - 1].Index, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1));
                        }
                        else if(_fPath != null)
                        {
                            pl1 = CalcPathLength(0, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            pl2 = CalcPathLength(_fPath.PointCount - (_fPath.PointCount - dg.SelectedRows[cnt - 1].Index), dg.Rows.Count - 1, _fPath.PathPoints, true);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1 + pl2));
                        }

                        double[] newMappings = new double[] { };

                        if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                        {
                            //if (this._useNormalDist)
                            //    newMappings = RestrictOrProlongatePoints(this._mappings, plG);
                            //else
                            {
                                newMappings = RestrictOrProlongatePoints(this._mappings, plG / 2 + 2);

                                double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                newMappings.CopyTo(tmp, 0);
                                List<double> lTmp = newMappings.ToList();
                                lTmp.RemoveAt(lTmp.Count - 1);
                                lTmp.Reverse();
                                lTmp.CopyTo(tmp, lTmp.Count + 1);

                                newMappings = tmp;
                            }

                            this._batchEdit = true;
                            double cpl = 0;

                            if (dg.SelectedRows[cnt - 1].Index > dg.SelectedRows[0].Index)
                            {
                                for (int j = 0; j <= dg.SelectedRows[0].Index; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(cpl + pl2), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["x"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["x"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }

                                cpl = 0;

                                for (int j = dg.SelectedRows[cnt - 1].Index; j <= dg.Rows.Count - 1; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && j > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(plG - cpl), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["x"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["x"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }
                            }
                            else
                                for (int i = dg.SelectedRows[cnt - 1].Index; i <= dg.SelectedRows[0].Index; i++)
                                {
                                    if ((dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                    {
                                        double curDist = 0;
                                        if (i > 0 && i > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                        {
                                            double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                            double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                            curDist = Math.Sqrt(dx * dx + dy * dy);
                                            cpl += curDist;
                                        }

                                        int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(pl1 - cpl), newMappings.Count() - 1), 0);

                                        string? s = dg.Rows[i].Cells["x"].Value.ToString();
                                        if (s != null)
                                            dg.Rows[i].Cells["x"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                    }
                                }
                            this._batchEdit = false;
                        }
                        else
                        {
                            this._batchEdit = true;
                            foreach (DataGridViewRow dr in dg.SelectedRows)
                            {
                                if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                                {
                                    string? s = dr.Cells["x"].Value.ToString();
                                    if (s != null)
                                        dr.Cells["x"].Value = float.Parse(s) - System.Convert.ToSingle(this.numericUpDown2.Value);
                                }
                            }
                            this._batchEdit = false;
                        }

                        List<List<PointF>> pts = GetPointsFromDGV(dg);

                        SetupNewPath(pts);
                    }
                    catch
                    {
                    }
                }
            }
            else if (this._bloaded)
            {
                try
                {
                    DataGridView dg = this.DataGridView1;

                    int cnt = this.DataGridView1.Rows.Count;
                    double[] newMappings = new double[] { };

                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0 && _fPath != null)
                    {
                        //if (this._useNormalDist)
                        //    newMappings = RestrictOrProlongatePoints(this._mappings, cnt);
                        //else
                        {
                            newMappings = RestrictOrProlongatePoints(this._mappings, cnt / 2 + 2);

                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                            newMappings.CopyTo(tmp, 0);
                            List<double> lTmp = newMappings.ToList();
                            lTmp.RemoveAt(lTmp.Count - 1);
                            lTmp.Reverse();
                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                            newMappings = tmp;
                        }

                        this._batchEdit = true;
                        int i = 0;

                        double cpl = 0;
                        double pl1 = CalcPathLength(0, dg.Rows.Count - 1, _fPath.PathPoints.ToArray(), false);

                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                double curDist = 0;
                                if (i > 0)
                                {
                                    double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                    double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                    curDist = Math.Sqrt(dx * dx + dy * dy);
                                    cpl += curDist;
                                }

                                double v = cpl / pl1;
                                int mIndx = Math.Min(System.Convert.ToInt32((1.0 - v) * (newMappings.Count() - 1)), newMappings.Count() - 1);

                                string? s = dr.Cells["x"].Value.ToString();
                                if (s != null)
                                    dr.Cells["x"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                // dr.Cells["x"].Value = Single.Parse(dr.Cells["x"].Value.ToString()) - CSng(Me.numericUpDown2.Value * newMappings[i])
                                i += 1;
                            }
                        }
                        this._batchEdit = false;
                    }
                    else
                    {
                        this._batchEdit = true;
                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                string? s = dr.Cells["x"].Value.ToString();
                                if (s != null)
                                    dr.Cells["x"].Value = float.Parse(s) - System.Convert.ToSingle(this.numericUpDown2.Value);
                            }
                        }
                        this._batchEdit = false;
                    }

                    List<List<PointF>> pts = GetPointsFromDGV(dg);

                    SetupNewPath(pts);
                }
                catch
                {
                }
            }
            if (this.Timer1.Enabled)
                this.Timer1.Stop();
            this.Timer1.Start();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.CheckBox5.Checked)
            {
                if (this._bloaded)
                {
                    try
                    {
                        DataGridView dg = this.DataGridView1;

                        DataGridViewSelectedRowCollection sel = dg.SelectedRows;
                        int cnt = sel.Cast<DataGridViewRow>().Count();

                        // compute newMappings by pathLength instead of PointCount
                        double pl1 = 0;
                        double pl2 = 0;
                        int plG = 0;

                        if (dg.SelectedRows[cnt - 1].Index < dg.SelectedRows[0].Index && _fPath != null)
                        {
                            pl1 = CalcPathLength(dg.SelectedRows[cnt - 1].Index, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1));
                        }
                        else if(_fPath != null)
                        {
                            pl1 = CalcPathLength(0, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            pl2 = CalcPathLength(_fPath.PointCount - (_fPath.PointCount - dg.SelectedRows[cnt - 1].Index), dg.Rows.Count - 1, _fPath.PathPoints, true);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1 + pl2));
                        }

                        double[] newMappings = new double[] { };

                        if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                        {
                            //if (this._useNormalDist)
                            //    newMappings = RestrictOrProlongatePoints(this._mappings, plG);
                            //else
                            {
                                newMappings = RestrictOrProlongatePoints(this._mappings, plG / 2 + 2);

                                double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                newMappings.CopyTo(tmp, 0);
                                List<double> lTmp = newMappings.ToList();
                                lTmp.RemoveAt(lTmp.Count - 1);
                                lTmp.Reverse();
                                lTmp.CopyTo(tmp, lTmp.Count + 1);

                                newMappings = tmp;
                            }

                            this._batchEdit = true;
                            double cpl = 0;

                            if (dg.SelectedRows[cnt - 1].Index > dg.SelectedRows[0].Index)
                            {
                                for (int j = 0; j <= dg.SelectedRows[0].Index; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(cpl + pl2), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["y"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["y"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }

                                cpl = 0;

                                for (int j = dg.SelectedRows[cnt - 1].Index; j <= dg.Rows.Count - 1; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && j > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(plG - cpl), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["y"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["y"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }
                            }
                            else
                                for (int i = dg.SelectedRows[cnt - 1].Index; i <= dg.SelectedRows[0].Index; i++)
                                {
                                    if ((dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                    {
                                        double curDist = 0;
                                        if (i > 0 && i > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                        {
                                            double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                            double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                            curDist = Math.Sqrt(dx * dx + dy * dy);
                                            cpl += curDist;
                                        }

                                        int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(pl1 - cpl), newMappings.Count() - 1), 0);

                                        string? s = dg.Rows[i].Cells["y"].Value.ToString();
                                        if (s != null)
                                            dg.Rows[i].Cells["y"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                    }
                                }
                            this._batchEdit = false;
                        }
                        else
                        {
                            this._batchEdit = true;
                            foreach (DataGridViewRow dr in dg.SelectedRows)
                            {
                                if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                                {
                                    string? s = dr.Cells["y"].Value.ToString();
                                    if (s != null)
                                        dr.Cells["y"].Value = float.Parse(s) - System.Convert.ToSingle(this.numericUpDown2.Value);
                                }
                            }
                            this._batchEdit = false;
                        }

                        List<List<PointF>> pts = GetPointsFromDGV(dg);

                        SetupNewPath(pts);
                    }
                    catch
                    {
                    }
                }
            }
            else if (this._bloaded)
            {
                try
                {
                    DataGridView dg = this.DataGridView1;

                    int cnt = this.DataGridView1.Rows.Count;
                    double[] newMappings = new double[] { };

                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0 && _fPath != null)
                    {
                        //if (this._useNormalDist)
                        //    newMappings = RestrictOrProlongatePoints(this._mappings, cnt);
                        //else
                        {
                            newMappings = RestrictOrProlongatePoints(this._mappings, cnt / 2 + 2);

                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                            newMappings.CopyTo(tmp, 0);
                            List<double> lTmp = newMappings.ToList();
                            lTmp.RemoveAt(lTmp.Count - 1);
                            lTmp.Reverse();
                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                            newMappings = tmp;
                        }

                        this._batchEdit = true;
                        int i = 0;

                        double cpl = 0;
                        double pl1 = CalcPathLength(0, dg.Rows.Count - 1, _fPath.PathPoints.ToArray(), false);

                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                double curDist = 0;
                                if (i > 0)
                                {
                                    double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                    double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                    curDist = Math.Sqrt(dx * dx + dy * dy);
                                    cpl += curDist;
                                }

                                double v = cpl / pl1;
                                int mIndx = Math.Min(System.Convert.ToInt32((1.0 - v) * (newMappings.Count() - 1)), newMappings.Count() - 1);

                                string? s = dr.Cells["y"].Value.ToString();
                                if (s != null)
                                    dr.Cells["y"].Value = float.Parse(s) - System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                // dr.Cells["y"].Value = Single.Parse(dr.Cells["y"].Value.ToString()) - CSng(Me.numericUpDown2.Value * newMappings[i])
                                i += 1;
                            }
                        }
                        this._batchEdit = false;
                    }
                    else
                    {
                        this._batchEdit = true;
                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                string? s = dr.Cells["y"].Value.ToString();
                                if (s != null)
                                    dr.Cells["y"].Value = float.Parse(s) - System.Convert.ToSingle(this.numericUpDown2.Value);
                            }
                        }
                        this._batchEdit = false;
                    }

                    List<List<PointF>> pts = GetPointsFromDGV(dg);

                    SetupNewPath(pts);
                }
                catch
                {
                }
            }
            if (this.Timer1.Enabled)
                this.Timer1.Stop();
            this.Timer1.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (this.CheckBox5.Checked)
            {
                if (this._bloaded)
                {
                    try
                    {
                        DataGridView dg = this.DataGridView1;

                        DataGridViewSelectedRowCollection sel = dg.SelectedRows;
                        int cnt = sel.Cast<DataGridViewRow>().Count();

                        // compute newMappings by pathLength instead of PointCount
                        double pl1 = 0;
                        double pl2 = 0;
                        int plG = 0;

                        if (dg.SelectedRows[cnt - 1].Index < dg.SelectedRows[0].Index && _fPath != null)
                        {
                            pl1 = CalcPathLength(dg.SelectedRows[cnt - 1].Index, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1));
                        }
                        else if(_fPath != null)
                        {
                            pl1 = CalcPathLength(0, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                            pl2 = CalcPathLength(_fPath.PointCount - (_fPath.PointCount - dg.SelectedRows[cnt - 1].Index), dg.Rows.Count - 1, _fPath.PathPoints, true);
                            plG = System.Convert.ToInt32(Math.Ceiling(pl1 + pl2));
                        }

                        double[] newMappings = new double[] { };

                        if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                        {
                            //if (this._useNormalDist)
                            //    newMappings = RestrictOrProlongatePoints(this._mappings, plG);
                            //else
                            {
                                newMappings = RestrictOrProlongatePoints(this._mappings, plG / 2 + 2);

                                double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                newMappings.CopyTo(tmp, 0);
                                List<double> lTmp = newMappings.ToList();
                                lTmp.RemoveAt(lTmp.Count - 1);
                                lTmp.Reverse();
                                lTmp.CopyTo(tmp, lTmp.Count + 1);

                                newMappings = tmp;
                            }

                            this._batchEdit = true;
                            double cpl = 0;

                            if (dg.SelectedRows[cnt - 1].Index > dg.SelectedRows[0].Index)
                            {
                                for (int j = 0; j <= dg.SelectedRows[0].Index; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(cpl + pl2), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["y"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["y"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }

                                cpl = 0;

                                for (int j = dg.SelectedRows[cnt - 1].Index; j <= dg.Rows.Count - 1; j++)
                                {
                                    double curDist = 0;
                                    if (j > 0 && j > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                    {
                                        double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                        double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                        cpl += curDist;
                                    }

                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(plG - cpl), newMappings.Count() - 1), 0);

                                    string? s = dg.Rows[j].Cells["y"].Value.ToString();
                                    if (s != null)
                                        dg.Rows[j].Cells["y"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                }
                            }
                            else
                                for (int i = dg.SelectedRows[cnt - 1].Index; i <= dg.SelectedRows[0].Index; i++)
                                {
                                    if ((dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                    {
                                        double curDist = 0;
                                        if (i > 0 && i > dg.SelectedRows[cnt - 1].Index && _fPath != null)
                                        {
                                            double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                            double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                            curDist = Math.Sqrt(dx * dx + dy * dy);
                                            cpl += curDist;
                                        }

                                        int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(pl1 - cpl), newMappings.Count() - 1), 0);

                                        string? s = dg.Rows[i].Cells["y"].Value.ToString();
                                        if (s != null)
                                            dg.Rows[i].Cells["y"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                    }
                                }
                            this._batchEdit = false;
                        }
                        else
                        {
                            this._batchEdit = true;
                            foreach (DataGridViewRow dr in dg.SelectedRows)
                            {
                                if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                                {
                                    string? s = dr.Cells["y"].Value.ToString();
                                    if (s != null)
                                        dr.Cells["y"].Value = float.Parse(s) + System.Convert.ToSingle(this.numericUpDown2.Value);
                                }
                            }
                            this._batchEdit = false;
                        }

                        List<List<PointF>> pts = GetPointsFromDGV(dg);

                        SetupNewPath(pts);
                    }
                    catch
                    {
                    }
                }
            }
            else if (this._bloaded)
            {
                try
                {
                    DataGridView dg = this.DataGridView1;

                    int cnt = this.DataGridView1.Rows.Count;
                    double[] newMappings = new double[] { };

                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0 && _fPath != null)
                    {
                        //if (this._useNormalDist)
                        //    newMappings = RestrictOrProlongatePoints(this._mappings, cnt);
                        //else
                        {
                            newMappings = RestrictOrProlongatePoints(this._mappings, cnt / 2 + 2);

                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                            newMappings.CopyTo(tmp, 0);
                            List<double> lTmp = newMappings.ToList();
                            lTmp.RemoveAt(lTmp.Count - 1);
                            lTmp.Reverse();
                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                            newMappings = tmp;
                        }

                        this._batchEdit = true;
                        int i = 0;

                        double cpl = 0;
                        double pl1 = CalcPathLength(0, dg.Rows.Count - 1, _fPath.PathPoints.ToArray(), false);

                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                double curDist = 0;
                                if (i > 0)
                                {
                                    double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                    double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                    curDist = Math.Sqrt(dx * dx + dy * dy);
                                    cpl += curDist;
                                }

                                double v = cpl / pl1;
                                int mIndx = Math.Min(System.Convert.ToInt32((1.0 - v) * (newMappings.Count() - 1)), newMappings.Count() - 1);

                                string? s = dr.Cells["y"].Value.ToString();
                                if (s != null)
                                    dr.Cells["y"].Value = float.Parse(s) + System.Convert.ToSingle((double)this.numericUpDown2.Value * newMappings[mIndx]);
                                // dr.Cells["y"].Value = Single.Parse(dr.Cells["y"].Value.ToString()) + CSng(Me.numericUpDown2.Value * newMappings[i])
                                i += 1;
                            }
                        }
                        this._batchEdit = false;
                    }
                    else
                    {
                        this._batchEdit = true;
                        foreach (DataGridViewRow dr in dg.Rows)
                        {
                            if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                            {
                                string? s = dr.Cells["y"].Value.ToString();
                                if (s != null)
                                    dr.Cells["y"].Value = float.Parse(s) + System.Convert.ToSingle(this.numericUpDown2.Value);
                            }
                        }
                        this._batchEdit = false;
                    }

                    List<List<PointF>> pts = GetPointsFromDGV(dg);

                    SetupNewPath(pts);
                }
                catch
                {
                }
            }
            if (this.Timer1.Enabled)
                this.Timer1.Stop();
            this.Timer1.Start();
        }

        private void SetupNewPath(List<List<PointF>> pts)
        {
            using (GraphicsPath gPath = new GraphicsPath())
            {
                for (int i = 0; i <= pts.Count - 1; i++)
                {
                    gPath.StartFigure();
                    gPath.AddLines(pts[i].ToArray());
                    gPath.CloseFigure();
                }

                Image? iOld = this.PictureBox1.Image;
                if (_bmpBU != null && AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 4L))
                {
                    this.PictureBox1.Image = (Bitmap)this._bmpBU.Clone();

                    if (iOld != null)
                    {
                        iOld.Dispose();
                        iOld = null;
                    }

                    if (this.RadioButton2.Checked || this.RadioButton1.Checked)
                    {
                        using (Graphics g = Graphics.FromImage(this.PictureBox1.Image))
                        {
                            using (Pen p = new Pen(Color.Red, System.Convert.ToInt32(numericUpDown3.Value)))
                            {
                                p.LineJoin = LineJoin.Round;
                                g.DrawPath(p, gPath);
                            }
                        }
                    }

                    this.PictureBox1.Refresh();

                    GraphicsPath? pOld = _fPath;
                    _fPath = (GraphicsPath)gPath.Clone();
                    if (pOld != null)
                    {
                        pOld.Dispose();
                        pOld = null;
                    }

                    //_dirty = true;
                }
                else
                    MessageBox.Show("Not emnough Memory");
            }
        }

        private void DataGridView1_CellValueChanged(System.Object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (this._bloaded && !this._batchEdit)
            {
                try
                {
                    DataGridView dg = (DataGridView)sender;

                    List<List<PointF>> pts = GetPointsFromDGV(dg);

                    SetupNewPath(pts);
                }
                catch
                {
                }
            }
        }

        private List<List<PointF>> GetPointsFromDGV(DataGridView dg)
        {
            List<List<PointF>> pts = new List<List<PointF>>();
            List<PointF> ptI = new List<PointF>();

            int cnt = 0;
            int j = 0;
            try
            {
                foreach (DataGridViewRow dr in dg.Rows)
                {
                    if ((dr.Cells["x"].Value != null) && (dr.Cells["y"].Value != null))
                    {
                        string? s1 = dr.Cells["x"].Value.ToString();
                        string? s2 = dr.Cells["y"].Value.ToString();
                        if (s1 != null && s2 != null)
                        {
                            PointF pt = new PointF(float.Parse(s1), float.Parse(s2));

                            ptI.Add(pt);
                            j += 1;

                            if (this._newFigures != null && j > 0 && j <= dg.Rows.Count && cnt < this._newFigures.Count && j == this._newFigures[cnt])
                            {
                                cnt += 1;
                                pts.Add(ptI);
                                ptI = new List<PointF>();
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return pts;
        }

        private void DataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            try
            {
                DataGridView dg = (DataGridView)sender;
                if (e != null)
                {
                    if (dg.Columns[e.ColumnIndex].Name == "x" || dg.Columns[e.ColumnIndex].Name == "y")
                    {

                        if (e.Value != null)
                        {
                            try
                            {
                                string? s = e.Value.ToString();
                                if (s != null)
                                {
                                    e.Value = float.Parse(s);
                                    e.ParsingApplied = true;
                                }
                            }
                            catch
                            {
                                e.ParsingApplied = false;
                            }
                        }

                    }

                    if (dg.Columns[e.ColumnIndex].Name == "Nr")
                    {

                        if (e.Value != null)
                        {
                            try
                            {
                                string? s = e.Value.ToString();
                                if (s != null)
                                {
                                    e.Value = int.Parse(s);
                                    e.ParsingApplied = true;
                                }
                            }
                            catch
                            {
                                e.ParsingApplied = false;
                            }
                        }

                    }
                }
            }
            catch
            {
            }
        }

        private void DataGridView1_ColumnHeaderMouseClick(System.Object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;
            if (dg.Rows.Count > 0)
                DataGridView1_CellValueChanged(dg, new DataGridViewCellEventArgs(0, 0));
        }

        private void DataGridView1_RowsRemoved(System.Object sender, System.Windows.Forms.DataGridViewRowsRemovedEventArgs e)
        {

        }

        private void DisplayChanges()
        {
            if (this._bmpBU != null)
            {
                Image? iOld = this.PictureBox1.Image;

                if (AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 4L))
                {
                    this.PictureBox1.Image = (Bitmap)this._bmpBU.Clone();
                    if (iOld != null)
                    {
                        iOld.Dispose();
                        iOld = null;
                    }
                }
                else
                    MessageBox.Show("Not enough Memory");

                if (this.RadioButton2.Checked || this.RadioButton1.Checked)
                {
                    using (Graphics g = Graphics.FromImage(this.PictureBox1.Image))
                    {
                        using (Pen p = new Pen(Color.Red, System.Convert.ToInt32(numericUpDown3.Value)))
                        {
                            p.LineJoin = LineJoin.Round;
                            if (_fPath != null)
                                g.DrawPath(p, _fPath);
                        }
                    }
                }

                int i = 0;
                this.DataGridView1.Rows.Clear();

                if (this._fPath != null && this._fPath.PointCount > 1)
                {
                    foreach (PointF pt in _fPath.PathPoints)
                    {
                        i += 1;
                        this.DataGridView1.Rows.Add(i * 100, pt.X, pt.Y);
                    }
                }

                this.PictureBox1.Refresh();
            }
        }

        private void DataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int r = e.RowIndex;
            this._selectedRow = r;
            this._curPt = new PointF(-1, -1);
            this.PictureBox1.Refresh();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            this.SplitContainer2.Panel2.Focus();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (this.PictureBox1.Image != null && this._fPath != null && this._fPath.PointCount > 1)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                if (this.RadioButton1.Checked)
                {
                    PointF ptF = new PointF(-1, -1);
                    bool found = false;
                    int i = 0;
                    foreach (PointF pt in _fPath.PathPoints)
                    {
                        if (this._selectedRow > -1 && this._selectedRow < this.DataGridView1.Rows.Count)
                        {
                            float x = System.Convert.ToSingle(this.DataGridView1.Rows[this._selectedRow].Cells[1].Value);
                            float y = System.Convert.ToSingle(this.DataGridView1.Rows[this._selectedRow].Cells[2].Value);

                            if (found == false && (pt.X == x && pt.Y == y) || (pt.X == this._curPt.X && pt.Y == this._curPt.Y))
                            {
                                if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                                    e.Graphics.DrawEllipse(Pens.Lime, new RectangleF(pt.X - 6.0F, pt.Y - 6.0F, 12, 12));
                                else if (_r2 != null)
                                    e.Graphics.DrawEllipse(Pens.Lime, new RectangleF(_r2.Value.X + pt.X * _zoom - 6.0F, _r2.Value.Y + pt.Y * _zoom - 6.0F, 12, 12));
                                ptF = pt;
                                found = true;
                            }
                            else if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                                e.Graphics.FillEllipse(Brushes.Aqua, new RectangleF(pt.X - 6.0F, pt.Y - 6.0F, 12, 12));
                            else if (_r2 != null)
                                e.Graphics.FillEllipse(Brushes.Aqua, new RectangleF(_r2.Value.X + pt.X * _zoom - 6.0F, _r2.Value.Y + pt.Y * _zoom - 6.0F, 12, 12));
                        }
                        else if (found == false && pt.X == this._curPt.X && pt.Y == this._curPt.Y)
                        {
                            if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                                e.Graphics.DrawEllipse(Pens.Lime, new RectangleF(pt.X - 6.0F, pt.Y - 6.0F, 12, 12));
                            else if (_r2 != null)
                                e.Graphics.DrawEllipse(Pens.Lime, new RectangleF(_r2.Value.X + pt.X * _zoom - 6.0F, _r2.Value.Y + pt.Y * _zoom - 6.0F, 12, 12));
                            ptF = pt;
                            found = true;
                        }
                        else if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                            e.Graphics.FillEllipse(Brushes.Aqua, new RectangleF(pt.X - 6.0F, pt.Y - 6.0F, 12, 12));
                        else if (_r2 != null)
                            e.Graphics.FillEllipse(Brushes.Aqua, new RectangleF(_r2.Value.X + pt.X * _zoom - 6.0F, _r2.Value.Y + pt.Y * _zoom - 6.0F, 12, 12));
                        if (i < this.DataGridView1.Rows.Count && this.DataGridView1.SelectedRows.Contains(this.DataGridView1.Rows[i]))
                        {
                            using (Pen pen = new Pen(Color.Yellow, 2))
                            {
                                if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                                    e.Graphics.DrawRectangle(pen, pt.X - 6.0F, pt.Y - 6.0F, 12, 12);
                                else if (_r2 != null)
                                    e.Graphics.DrawRectangle(pen, _r2.Value.X + pt.X * _zoom - 6.0F, _r2.Value.Y + pt.Y * _zoom - 6.0F, 12, 12);
                            }
                        }
                        i += 1;
                    }
                    if (ptF.X > -1 && ptF.Y > -1)
                    {
                        using (Pen pen = new Pen(Color.Black, 2))
                        {
                            if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                                e.Graphics.DrawEllipse(pen, new RectangleF(ptF.X - 8.0F, ptF.Y - 8.0F, 16, 16));
                            else if (_r2 != null)
                                e.Graphics.DrawEllipse(pen, new RectangleF(_r2.Value.X + ptF.X * _zoom - 8.0F, _r2.Value.Y + ptF.Y * _zoom - 8.0F, 16, 16));
                        }
                    }

                    if (this._movedPoint.X > -1 && this._movedPoint.Y > -1)
                    {
                        PointF ptJa = new PointF((this._movedPoint.X - this._curPt.X) - _offsetX, (this._movedPoint.Y - this._curPt.Y) - _offsetY);
                        if (this.PictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
                        {
                            e.Graphics.DrawLine(Pens.Red, this._curPt, ptJa);
                            e.Graphics.DrawEllipse(Pens.OrangeRed, new RectangleF(ptJa.X - 8.0F, ptJa.Y - 8.0F, 16, 16));
                        }
                        else if (_r2 != null)
                        {
                            e.Graphics.DrawLine(Pens.Red, _r2.Value.X + this._curPt.X * _zoom, _r2.Value.Y + this._curPt.Y * _zoom, _r2.Value.X + ptJa.X * _zoom, _r2.Value.Y + ptJa.Y * _zoom);
                            e.Graphics.DrawEllipse(Pens.OrangeRed, new RectangleF(_r2.Value.X + ptJa.X * _zoom - 8.0F, _r2.Value.Y + ptJa.Y * _zoom - 8.0F, 16, 16));
                        }
                    }
                }
            }
        }

        // get position of pic in picturebox
        private Rectangle? GetImageRectangle()
        {
            Type pboxType = this.PictureBox1.GetType();

            if (pboxType != null)
            {
                System.Reflection.PropertyInfo? irProperty = pboxType.GetProperty("ImageRectangle", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                Rectangle? r = null;

                if (irProperty != null)
                    r = (Rectangle?)irProperty.GetValue(this.PictureBox1, null);

                _zoom = System.Convert.ToSingle(r?.Width / (double)this.PictureBox1.Image.Width);

                if (r != null)
                    return r.Value;
            }

            return null;
        }

        private void PictureBox1_MouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            int x = 0;
            int y = 0;

            if (pic.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                this.ToolStripStatusLabel1.Text = e.X.ToString() + ";" + e.Y.ToString();
                x = e.X;
                y = e.Y;
            }
            else
            {
                double d1 = System.Convert.ToDouble(pic.Image.Width) / System.Convert.ToDouble(pic.Image.Height);
                double d2 = System.Convert.ToDouble(pic.Width) / System.Convert.ToDouble(pic.Height);

                if (d1 > d2)
                {
                    double d = System.Convert.ToDouble(pic.Width) / System.Convert.ToDouble(pic.Image.Width);
                    x = Convert.ToInt32(e.X / d);
                    y = Convert.ToInt32((e.Y - (((System.Convert.ToDouble(pic.Height)) - (pic.Image.Height * d)) / 2.0)) / d);
                }
                else
                {
                    double d = System.Convert.ToDouble(pic.Height) / System.Convert.ToDouble(pic.Image.Height);
                    x = Convert.ToInt32((e.X - (((System.Convert.ToDouble(pic.Width)) - (pic.Image.Width * d)) / 2.0)) / d);
                    y = Convert.ToInt32(e.Y / d);
                }

                this.ToolStripStatusLabel1.Text = x.ToString() + ";" + y.ToString();
            }

            if (e.Button == MouseButtons.Left)
            {
                if (this._curPt.X > -1 && this._curPt.Y > -1 && _fPath != null && _fPath.PointCount > 1)
                {
                    int dx = x - _ix;
                    int dy = y - _iy;

                    this._movedPoint = new PointF(_ix + x - _offsetX, _iy + y - _offsetY);

                    this.PictureBox1.Invalidate();
                }
            }
        }

        private void CheckBox1_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                this.PictureBox1.Dock = DockStyle.Fill;
                this.PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                this.PictureBox1.Dock = DockStyle.None;
                this.PictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }

            if (this.PictureBox1.Image != null)
            {
                this._r2 = GetImageRectangle();
                this.PictureBox1.Invalidate();
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int ix = -1;
            int iy = -1;
            this._curPt = new PointF(-1, -1);
            this._movedPoint = new Point(-1, -1);
            this._offsetX = -1;
            this._offsetY = -1;
            PictureBox pic = (PictureBox)sender;

            if (pic.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                ix = e.X;
                iy = e.Y;
            }
            else
            {
                double d1 = Convert.ToDouble(pic.Image.Width) / Convert.ToDouble(pic.Image.Height);
                double d2 = Convert.ToDouble(pic.Width) / Convert.ToDouble(pic.Height);

                int x = 0;
                int y = 0;

                if (d1 > d2)
                {
                    double d = Convert.ToDouble(pic.Width) / Convert.ToDouble(pic.Image.Width);
                    x = Convert.ToInt32(e.X / d);
                    y = Convert.ToInt32(((e.Y - (((Convert.ToDouble(pic.Height)) - (pic.Image.Height * d)) / 2.0)) / d));
                }
                else
                {
                    double d = Convert.ToDouble(pic.Height) / Convert.ToDouble(pic.Image.Height);
                    x = Convert.ToInt32(((e.X - (((Convert.ToDouble(pic.Width)) - (pic.Image.Width * d)) / 2.0)) / d));
                    y = Convert.ToInt32(e.Y / d);
                }

                ix = x;
                iy = y;
            }

            _ix = ix;
            _iy = iy;

            this.ToolStripStatusLabel1.Text = x.ToString() + ";" + y.ToString();

            if (e.Button == MouseButtons.Left)
            {
                if (ix > -1 && iy > -1 && _fPath != null && _fPath.PointCount > 1)
                {
                    this.Enabled = false;
                    PointF pt = new PointF(-1, -1);
                    for (int i = 0; i <= this._fPath.PathPoints.Length - 1; i++)
                    {
                        if (Math.Abs(_fPath.PathPoints[i].X - ix) < 5 / (double)_zoom && Math.Abs(_fPath.PathPoints[i].Y - iy) < 5 / (double)_zoom)
                            pt = _fPath.PathPoints[i];
                    }
                    if (pt.X > -1 && pt.Y > -1)
                    {
                        for (int i = 0; i <= this.DataGridView1.Rows.Count - 1; i++)
                        {
                            if (Math.Floor(System.Convert.ToSingle(this.DataGridView1.Rows[i].Cells[1].Value)) == Math.Floor(pt.X) && Math.Floor(System.Convert.ToSingle(this.DataGridView1.Rows[i].Cells[2].Value)) == Math.Floor(pt.Y))
                            {
                                this.DataGridView1.ClearSelection();
                                this.DataGridView1.Rows[i].Selected = true;
                                this.DataGridView1.FirstDisplayedScrollingRowIndex = DataGridView1.SelectedRows[0].Index;
                            }
                        }
                    }
                    this._curPt = pt;
                    this._offsetX = ix - pt.X;
                    this._offsetY = iy - pt.Y;
                    this.Enabled = true;
                    this.PictureBox1.Invalidate();
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.Timer1.Stop();
            this.PictureBox1.Refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DataGridView dg = this.DataGridView1;

            DataGridViewSelectedRowCollection sel = dg.SelectedRows;

            if (sel.Count > 0)
            {
                int i = dg.SelectedRows[0].Index;
                dg.ClearSelection();

                if (i > -1)
                {
                    this.Label1.Text = i.ToString();
                    CheckAndSelect();
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DataGridView dg = this.DataGridView1;

            DataGridViewSelectedRowCollection sel = dg.SelectedRows;

            if (sel.Count > 0)
            {
                int i = dg.SelectedRows[sel.Count - 1].Index;
                dg.ClearSelection();

                if (i > -1)
                {
                    this.Label2.Text = i.ToString();
                    CheckAndSelect();
                }
            }
        }

        private void CheckAndSelect()
        {
            string s1 = this.Label1.Text;
            string s2 = this.Label2.Text;

            int st = -1;
            int ed = -1;

            if (Int32.TryParse(s1, out st))
            {
                if (Int32.TryParse(s2, out ed))
                {
                    if (st != ed)
                    {
                        DataGridView dg = this.DataGridView1;

                        if (st > ed)
                        {
                            // Dim tmp As Integer = st 'old version
                            // st = ed
                            // ed = tmp

                            dg.ClearSelection();

                            for (int i = st; i <= dg.Rows.Count - 1; i++)
                                dg.Rows[i].Selected = true;
                            for (int i = 0; i <= ed; i++)
                                dg.Rows[i].Selected = true;
                        }
                        else
                        {
                            dg.ClearSelection();

                            for (int i = st; i <= ed; i++)
                                dg.Rows[i].Selected = true;
                        }

                        this.CheckBox5.Checked = true;
                        dg.FirstDisplayedScrollingRowIndex = st;

                        this.PictureBox1.Invalidate();
                    }
                }
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            int x = 0;
            int y = 0;

            if (pic.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                this.ToolStripStatusLabel1.Text = e.X.ToString() + ";" + e.Y.ToString();
                x = e.X;
                y = e.Y;
            }
            else
            {
                double d1 = System.Convert.ToDouble(pic.Image.Width) / System.Convert.ToDouble(pic.Image.Height);
                double d2 = System.Convert.ToDouble(pic.Width) / System.Convert.ToDouble(pic.Height);

                if (d1 > d2)
                {
                    double d = System.Convert.ToDouble(pic.Width) / System.Convert.ToDouble(pic.Image.Width);
                    x = Convert.ToInt32(e.X / d);
                    y = Convert.ToInt32((e.Y - (((System.Convert.ToDouble(pic.Height)) - (pic.Image.Height * d)) / 2.0)) / d);
                }
                else
                {
                    double d = System.Convert.ToDouble(pic.Height) / System.Convert.ToDouble(pic.Image.Height);
                    x = Convert.ToInt32((e.X - (((System.Convert.ToDouble(pic.Width)) - (pic.Image.Width * d)) / 2.0)) / d);
                    y = Convert.ToInt32(e.Y / d);
                }

                this.ToolStripStatusLabel1.Text = x.ToString() + ";" + y.ToString();
            }

            if (e.Button == MouseButtons.Left)
            {
                if (this._curPt.X > -1 && this._curPt.Y > -1 && _fPath != null && _fPath.PointCount > 1)
                {
                    int dx = x - _ix;
                    int dy = y - _iy;

                    this.Enabled = false;
                    PointF pt = this._curPt; // New PointF(-1, -1)
                    for (int i = 0; i <= this._fPath.PathPoints.Length - 1; i++)
                    {
                        if (Math.Abs(_fPath.PathPoints[i].X - x) < 5 / (double)_zoom && Math.Abs(_fPath.PathPoints[i].Y - y) < 5 / (double)_zoom)
                            pt = _fPath.PathPoints[i];
                    }
                    if (pt.X > -1 && pt.Y > -1)
                    {
                        for (int i = 0; i <= this.DataGridView1.Rows.Count - 1; i++)
                        {
                            if (Math.Floor(System.Convert.ToSingle(this.DataGridView1.Rows[i].Cells[1].Value)) == Math.Floor(pt.X) && Math.Floor(System.Convert.ToSingle(this.DataGridView1.Rows[i].Cells[2].Value)) == Math.Floor(pt.Y))
                            {
                                this.DataGridView1.ClearSelection();
                                this.DataGridView1.Rows[i].Selected = true;
                                this.DataGridView1.FirstDisplayedScrollingRowIndex = DataGridView1.SelectedRows[0].Index;

                                this.DataGridView1.Rows[i].Cells["x"].Value = System.Convert.ToSingle(this.DataGridView1.Rows[i].Cells["x"].Value) + dx;
                                this.DataGridView1.Rows[i].Cells["y"].Value = System.Convert.ToSingle(this.DataGridView1.Rows[i].Cells["y"].Value) + dy;
                            }
                        }
                    }
                    this.Enabled = true;
                    this.PictureBox1.Invalidate();
                }
            }

            this._movedPoint = new Point(-1, -1);
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            using (frmShiftInwards frm = new frmShiftInwards(this._shiftPathMode, this._shiftFractionMode))
            {
                frm.NumericUpDown1.Value = this.numericUpDown2.Value;
                frm.ComboBox1.SelectedIndex = System.Convert.ToInt32(this._shiftPathMode);
                frm.ComboBox2.SelectedIndex = System.Convert.ToInt32(this._shiftFractionMode);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.CheckBox2.Checked)
                    {
                        this.numericUpDown2.Value = frm.NumericUpDown1.Value;
                        if (this.CheckBox5.Checked)
                        {
                            if (this._bloaded)
                            {
                                try
                                {
                                    DataGridView dg = this.DataGridView1;

                                    List<List<PointF>> ptsIn = GetPointsFromDGV(dg);
                                    List<PointF> widenedPoints = GetWidenedPoints(ptsIn, -System.Convert.ToSingle(this.numericUpDown2.Value));

                                    DataGridViewSelectedRowCollection sel = dg.SelectedRows;
                                    int cnt = sel.Cast<DataGridViewRow>().Count();

                                    // compute newMappings by pathLength instead of PointCount
                                    double pl1 = 0;
                                    double pl2 = 0;
                                    int plG = 0;

                                    if (dg.SelectedRows[cnt - 1].Index < dg.SelectedRows[0].Index && _fPath != null)
                                    {
                                        pl1 = CalcPathLength(dg.SelectedRows[cnt - 1].Index, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                                        plG = System.Convert.ToInt32(Math.Ceiling(pl1));
                                    }
                                    else if(_fPath != null)
                                    {
                                        pl1 = CalcPathLength(0, dg.SelectedRows[0].Index, _fPath.PathPoints.ToArray(), false);
                                        pl2 = CalcPathLength(_fPath.PointCount - (_fPath.PointCount - dg.SelectedRows[cnt - 1].Index), dg.Rows.Count - 1, _fPath.PathPoints, true);
                                        plG = System.Convert.ToInt32(Math.Ceiling(pl1 + pl2));
                                    }

                                    double[] newMappings = new double[] { };

                                    int f = dg.SelectedRows[dg.SelectedRows.Cast<DataGridViewRow>().Count() - 1].Index;

                                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                                    {
                                        //if (this._useNormalDist)
                                        //    newMappings = RestrictOrProlongatePoints(this._mappings, plG);
                                        //else
                                        {
                                            newMappings = RestrictOrProlongatePoints(this._mappings, plG / 2 + 2);

                                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                            newMappings.CopyTo(tmp, 0);
                                            List<double> lTmp = newMappings.ToList();
                                            lTmp.RemoveAt(lTmp.Count - 1);
                                            lTmp.Reverse();
                                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                                            newMappings = tmp;
                                        }

                                        this._batchEdit = true;

                                        double cpl = 0;

                                        if (dg.SelectedRows[cnt - 1].Index > dg.SelectedRows[0].Index && _fPath != null)
                                        {
                                            for (int j = 0; j <= dg.SelectedRows[0].Index; j++)
                                            {
                                                double curDist = 0;
                                                if (j > 0)
                                                {
                                                    double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                                    double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                                    curDist = Math.Sqrt(dx * dx + dy * dy);
                                                    cpl += curDist;
                                                }

                                                int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(cpl + pl2), newMappings.Count() - 1), 0);

                                                float oldPtX = _fPath.PathPoints[j].X;
                                                float oldPtY = _fPath.PathPoints[j].Y;

                                                PointF newPt = widenedPoints[j]; // Mod dg.Rows.Count)

                                                float newPtX = newPt.X;
                                                float newPtY = newPt.Y;

                                                double shiftX = -(oldPtX - newPtX) * newMappings[mIndx];
                                                double shiftY = -(oldPtY - newPtY) * newMappings[mIndx];

                                                dg.Rows[j].Cells["x"].Value = System.Convert.ToSingle(oldPtX + shiftX);
                                                dg.Rows[j].Cells["y"].Value = System.Convert.ToSingle(oldPtY + shiftY);
                                            }

                                            cpl = 0;

                                            for (int j = dg.SelectedRows[cnt - 1].Index; j <= dg.Rows.Count - 1; j++)
                                            {
                                                double curDist = 0;
                                                if (j > 0 && j > dg.SelectedRows[cnt - 1].Index)
                                                {
                                                    double dx = _fPath.PathPoints[j].X - _fPath.PathPoints[j - 1].X;
                                                    double dy = _fPath.PathPoints[j].Y - _fPath.PathPoints[j - 1].Y;
                                                    curDist = Math.Sqrt(dx * dx + dy * dy);
                                                    cpl += curDist;
                                                }

                                                int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(plG - cpl), newMappings.Count() - 1), 0);

                                                float oldPtX = _fPath.PathPoints[j].X;
                                                float oldPtY = _fPath.PathPoints[j].Y;

                                                PointF newPt = widenedPoints[j]; // Mod dg.Rows.Count)

                                                float newPtX = newPt.X;
                                                float newPtY = newPt.Y;

                                                double shiftX = -(oldPtX - newPtX) * newMappings[mIndx];
                                                double shiftY = -(oldPtY - newPtY) * newMappings[mIndx];

                                                dg.Rows[j].Cells["x"].Value = System.Convert.ToSingle(oldPtX + shiftX);
                                                dg.Rows[j].Cells["y"].Value = System.Convert.ToSingle(oldPtY + shiftY);
                                            }
                                        }
                                        else
                                            for (int i = dg.SelectedRows[cnt - 1].Index; i <= dg.SelectedRows[0].Index; i++)
                                            {
                                                if (_fPath != null && (dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                                {
                                                    double curDist = 0;
                                                    if (i > 0 && i > dg.SelectedRows[cnt - 1].Index)
                                                    {
                                                        double dx = _fPath.PathPoints[i].X - _fPath.PathPoints[i - 1].X;
                                                        double dy = _fPath.PathPoints[i].Y - _fPath.PathPoints[i - 1].Y;
                                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                                        cpl += curDist;
                                                    }

                                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(pl1 - cpl), newMappings.Count() - 1), 0);

                                                    float oldPtX = _fPath.PathPoints[i].X;
                                                    float oldPtY = _fPath.PathPoints[i].Y;

                                                    PointF newPt = widenedPoints[i]; // Mod dg.Rows.Count)

                                                    float newPtX = newPt.X;
                                                    float newPtY = newPt.Y;

                                                    double shiftX = -(oldPtX - newPtX) * newMappings[mIndx];
                                                    double shiftY = -(oldPtY - newPtY) * newMappings[mIndx];

                                                    dg.Rows[i].Cells["x"].Value = System.Convert.ToSingle(oldPtX + shiftX);
                                                    dg.Rows[i].Cells["y"].Value = System.Convert.ToSingle(oldPtY + shiftY);
                                                }
                                            }

                                        // Dim sc = dg.SelectedRows.Cast(Of DataGridViewRow).Count - 1

                                        // For i As Integer = 0 To sc
                                        // If (Not dg.SelectedRows[i].Cells["x"].Value Is Nothing) AndAlso (Not dg.SelectedRows[i].Cells["y"].Value Is Nothing) Then
                                        // Dim oldPtX As Single = Single.Parse(dg.SelectedRows[sc - i].Cells["x"].Value.ToString())
                                        // Dim oldPtY As Single = Single.Parse(dg.SelectedRows[sc - i].Cells["y"].Value.ToString())

                                        // Dim newPt As PointF = widenedPoints((f + i) Mod dg.Rows.Count)

                                        // Dim newPtX As Single = newPt.X
                                        // Dim newPtY As Single = newPt.Y

                                        // Dim shiftX As Double = -(oldPtX - newPtX) * newMappings[i]
                                        // Dim shiftY As Double = -(oldPtY - newPtY) * newMappings[i]

                                        // dg.SelectedRows[sc - i].Cells["x"].Value = CSng(oldPtX + shiftX)
                                        // dg.SelectedRows[sc - i].Cells["y"].Value = CSng(oldPtY + shiftY)

                                        // End If
                                        // Next
                                        this._batchEdit = false;
                                    }
                                    else
                                    {
                                        this._batchEdit = true;
                                        var sc = dg.SelectedRows.Cast<DataGridViewRow>().Count() - 1;

                                        for (int i = 0; i <= sc; i++)
                                        {
                                            if ((dg.SelectedRows[i].Cells["x"].Value != null) && (dg.SelectedRows[i].Cells["y"].Value != null))
                                            {
                                                PointF newPt = widenedPoints[(f + i) % dg.Rows.Count];

                                                dg.SelectedRows[sc - i].Cells["x"].Value = newPt.X;
                                                dg.SelectedRows[sc - i].Cells["y"].Value = newPt.Y;
                                            }
                                        }
                                        this._batchEdit = false;
                                    }

                                    List<List<PointF>> pts = GetPointsFromDGV(dg);

                                    SetupNewPath(pts);
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc.Message);
                                }
                            }
                        }
                        else if (this._bloaded)
                        {
                            try
                            {
                                DataGridView dg = this.DataGridView1;

                                List<List<PointF>> ptsIn = GetPointsFromDGV(dg);
                                List<PointF> widenedPoints = GetWidenedPoints(ptsIn, -System.Convert.ToSingle(this.numericUpDown2.Value));

                                int cnt = this.DataGridView1.Rows.Count;
                                double[] newMappings = new double[] { };

                                if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                                {
                                    //if (this._useNormalDist)
                                    //    newMappings = RestrictOrProlongatePoints(this._mappings, cnt);
                                    //else
                                    {
                                        newMappings = RestrictOrProlongatePoints(this._mappings, cnt / 2 + 2);

                                        double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                        newMappings.CopyTo(tmp, 0);
                                        List<double> lTmp = newMappings.ToList();
                                        lTmp.RemoveAt(lTmp.Count - 1);
                                        lTmp.Reverse();
                                        lTmp.CopyTo(tmp, lTmp.Count + 1);

                                        newMappings = tmp;
                                    }

                                    this._batchEdit = true;
                                    var sc = dg.Rows.Count - 1;

                                    for (int i = 0; i <= sc; i++)
                                    {
                                        if ((dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                        {
                                            string? s1 = dg.Rows[i].Cells["x"].Value.ToString();
                                            string? s2 = dg.Rows[i].Cells["y"].Value.ToString();

                                            if (s1 != null && s2 != null)
                                            {
                                                float oldPtX = float.Parse(s1);
                                                float oldPtY = float.Parse(s2);

                                                PointF newPt = widenedPoints[i];

                                                float newPtX = newPt.X;
                                                float newPtY = newPt.Y;

                                                double shiftX = -(oldPtX - newPtX) * newMappings[i];
                                                double shiftY = -(oldPtY - newPtY) * newMappings[i];

                                                dg.Rows[i].Cells["x"].Value = System.Convert.ToSingle(oldPtX + shiftX);
                                                dg.Rows[i].Cells["y"].Value = System.Convert.ToSingle(oldPtY + shiftY);
                                            }
                                        }
                                    }
                                    this._batchEdit = false;
                                }
                                else
                                {
                                    this._batchEdit = true;
                                    var sc = dg.Rows.Count - 1;

                                    for (int i = 0; i <= sc; i++)
                                    {
                                        if ((dg.Rows[i].Cells["x"].Value != null) && (dg.Rows[i].Cells["y"].Value != null))
                                        {
                                            PointF newPt = widenedPoints[i];

                                            dg.Rows[i].Cells["x"].Value = newPt.X;
                                            dg.Rows[i].Cells["y"].Value = newPt.Y;
                                        }
                                    }
                                    this._batchEdit = false;
                                }

                                List<List<PointF>> pts = GetPointsFromDGV(dg);
                                SetupNewPath(pts);
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.Message);
                            }
                        }

                        if (this.Timer1.Enabled)
                            this.Timer1.Stop();
                        this.Timer1.Start();
                    }
                    else if (_fPath != null)
                        using (GraphicsPath lp = (GraphicsPath)_fPath.Clone())
                        {
                            this.numericUpDown2.Value = frm.NumericUpDown1.Value;
                            string? s1 = frm.ComboBox1.SelectedItem?.ToString();
                            string? s2 = frm.ComboBox2.SelectedItem?.ToString();

                            if (s1 != null && s2 != null)
                            {
                                this._shiftPathMode = (ShiftPathMode)System.Enum.Parse(typeof(ShiftPathMode), s1);
                                this._shiftFractionMode = (ShiftFractionMode)System.Enum.Parse(typeof(ShiftFractionMode), s2);
                                if (this._shiftPathMode == ShiftPathMode.DrawPath)
                                {
                                    using (GraphicsPath fPath = GetPath2(frm.CheckBox1.Checked))
                                    {
                                        if (fPath != null)
                                        {
                                            GraphicsPath? alfOld = _shiftedPath;
                                            _shiftedPath = (GraphicsPath)fPath.Clone();
                                            if (alfOld != null)
                                            {
                                                alfOld.Dispose();
                                                alfOld = null;
                                            }
                                        }
                                    }
                                }
                                else if (this._shiftPathMode == ShiftPathMode.FillPath)
                                {
                                    using (GraphicsPath fPath = GetPath(frm.CheckBox1.Checked))
                                    {
                                        if (fPath != null)
                                        {
                                            GraphicsPath? alfOld = _shiftedPath;
                                            _shiftedPath = (GraphicsPath)fPath.Clone();
                                            if (alfOld != null)
                                            {
                                                alfOld.Dispose();
                                                alfOld = null;
                                            }
                                        }
                                    }
                                }
                                else
                                    using (GraphicsPath fPath = GetPath4(frm.CheckBox1.Checked))
                                    {
                                        if (fPath != null)
                                        {
                                            GraphicsPath? alfOld = _shiftedPath;
                                            _shiftedPath = (GraphicsPath)fPath.Clone();
                                            if (alfOld != null)
                                            {
                                                alfOld.Dispose();
                                                alfOld = null;
                                            }
                                        }
                                    }
                            }
                            if (_shiftedPath != null && _shiftedPath.PointCount > 0)
                            {
                                if (CheckBox2.Checked)
                                    _shiftedPath.Reverse();

                                List<PointF> shiftedPoints = new List<PointF>();
                                shiftedPoints.AddRange(_shiftedPath.PathPoints);

                                // quasi autocorrelation
                                PointF startPt = lp.PathPoints[0];
                                int clI = -1;
                                double minDist = double.MaxValue;

                                for (int i = 0; i <= shiftedPoints.Count - 1; i++)
                                {
                                    double dx = startPt.X - shiftedPoints[i].X;
                                    double dy = startPt.Y - shiftedPoints[i].Y;

                                    double dist = Math.Sqrt(dx * dx + dy * dy);

                                    if (dist < minDist)
                                    {
                                        clI = i;
                                        minDist = dist;
                                    }
                                }

                                List<PointF> sPPP = new List<PointF>();

                                for (int i = clI; i <= shiftedPoints.Count - 1; i++)
                                    sPPP.Add(shiftedPoints[i]);
                                for (int i = 0; i <= clI - 1; i++)
                                    sPPP.Add(shiftedPoints[i]);

                                DataGridView dg = this.DataGridView1;
                                DataGridViewSelectedRowCollection sel = dg.SelectedRows;
                                int cnt = sel.Cast<DataGridViewRow>().Count();

                                List<PointF> lO = new List<PointF>();

                                if (this.CheckBox5.Checked && cnt > 0)
                                {
                                    // get startindx
                                    PointF startPt2 = lp.PathPoints[dg.SelectedRows[cnt - 1].Index]; // GetWidenedPoint(lp.PathPoints, dg.SelectedRows[cnt - 1].Index, -CSng(Me.numericUpDown2.Value)) ' lp.PathPoints[dg.SelectedRows[cnt - 1].Index]
                                    PointF startPt22 = GetWidenedPoint(lp.PathPoints, dg.SelectedRows[cnt - 1].Index, -System.Convert.ToSingle(this.numericUpDown2.Value));
                                    int clI2 = -1;
                                    double minDist2 = double.MaxValue;
                                    int clI22 = -1;
                                    double minDist22 = double.MaxValue;

                                    for (int i = 0; i <= sPPP.Count - 1; i++)
                                    {
                                        double dx = startPt2.X - sPPP[i].X;
                                        double dy = startPt2.Y - sPPP[i].Y;

                                        double dist = Math.Sqrt(dx * dx + dy * dy);

                                        if (dist < minDist2)
                                        {
                                            clI2 = i;
                                            minDist2 = dist;
                                        }

                                        double dx2 = startPt22.X - sPPP[i].X;
                                        double dy2 = startPt22.Y - sPPP[i].Y;

                                        double dist2 = Math.Sqrt(dx * dx + dy * dy);

                                        if (dist2 < minDist22)
                                        {
                                            clI22 = i;
                                            minDist22 = dist2;
                                        }
                                    }

                                    if (clI2 != clI22)
                                    {
                                        int signX = 0;
                                        int signY = 0;

                                        int signX2 = 0;
                                        int signY2 = 0;

                                        int signX4 = 0;
                                        int signY4 = 0;

                                        signX = Math.Sign(lp.PathPoints[dg.SelectedRows[cnt - 1].Index].X - lp.PathPoints[(dg.SelectedRows[cnt - 1].Index - 1) % lp.PointCount].X);
                                        signY = Math.Sign(lp.PathPoints[dg.SelectedRows[cnt - 1].Index].Y - lp.PathPoints[(dg.SelectedRows[cnt - 1].Index - 1) % lp.PointCount].Y);

                                        signX2 = Math.Sign(sPPP[clI2].X - sPPP[(clI2 - 1) % sPPP.Count].X);
                                        signY2 = Math.Sign(sPPP[clI2].Y - sPPP[(clI2 - 1) % sPPP.Count].Y);

                                        signX4 = Math.Sign(sPPP[clI22].X - sPPP[(clI22 - 1) % sPPP.Count].X);
                                        signY4 = Math.Sign(sPPP[clI22].Y - sPPP[(clI22 - 1) % sPPP.Count].Y);

                                        //if (signX == signX2 && signY == signY2)
                                        //    clI2 = clI2;
                                        //else if (signX == signX4 && signY == signY4)
                                        //    clI2 = clI22;
                                        //else if (signX == signX2 || signY == signY2)
                                        //    clI2 = clI2;
                                        //else 
                                        if (signX == signX4 || signY == signY4)
                                            clI2 = clI22;
                                    }

                                    // get endindx
                                    PointF endPt4 = lp.PathPoints[dg.SelectedRows[0].Index]; // GetWidenedPoint(lp.PathPoints, dg.SelectedRows[0].Index, -CSng(Me.numericUpDown2.Value)) ' lp.PathPoints(dg.SelectedRows[0].Index)
                                    PointF endPt44 = GetWidenedPoint(lp.PathPoints, dg.SelectedRows[0].Index, -System.Convert.ToSingle(this.numericUpDown2.Value));
                                    int clI4 = -1;
                                    double minDist4 = double.MaxValue;
                                    int clI44 = -1;
                                    double minDist44 = double.MaxValue;

                                    for (int i = 0; i <= sPPP.Count - 1; i++)
                                    {
                                        double dx = endPt4.X - sPPP[i].X;
                                        double dy = endPt4.Y - sPPP[i].Y;

                                        double dist = Math.Sqrt(dx * dx + dy * dy);

                                        if (dist < minDist4)
                                        {
                                            clI4 = i;
                                            minDist4 = dist;
                                        }

                                        double dx4 = endPt44.X - sPPP[i].X;
                                        double dy4 = endPt44.Y - sPPP[i].Y;

                                        double dist4 = Math.Sqrt(dx * dx + dy * dy);

                                        if (dist4 < minDist44)
                                        {
                                            clI44 = i;
                                            minDist44 = dist4;
                                        }
                                    }

                                    if (clI4 != clI44)
                                    {
                                        int signX = 0;
                                        int signY = 0;

                                        int signX2 = 0;
                                        int signY2 = 0;

                                        int signX4 = 0;
                                        int signY4 = 0;

                                        signX = Math.Sign(lp.PathPoints[dg.SelectedRows[0].Index].X - lp.PathPoints[(dg.SelectedRows[0].Index - 1) % lp.PointCount].X);
                                        signY = Math.Sign(lp.PathPoints[dg.SelectedRows[0].Index].Y - lp.PathPoints[(dg.SelectedRows[0].Index - 1) % lp.PointCount].Y);

                                        signX2 = Math.Sign(sPPP[clI4].X - sPPP[(clI4 - 1) % sPPP.Count].X);
                                        signY2 = Math.Sign(sPPP[clI4].Y - sPPP[(clI4 - 1) % sPPP.Count].Y);

                                        signX4 = Math.Sign(sPPP[clI44].X - sPPP[(clI44 - 1) % sPPP.Count].X);
                                        signY4 = Math.Sign(sPPP[clI44].Y - sPPP[(clI44 - 1) % sPPP.Count].Y);

                                        //if (signX == signX2 && signY == signY2)
                                        //    clI4 = clI4;
                                        //else if (signX == signX4 && signY == signY4)
                                        //    clI4 = clI44;
                                        //else if (signX == signX2 || signY == signY2)
                                        //    clI4 = clI4;
                                        //else 
                                        if (signX == signX4 || signY == signY4)
                                            clI4 = clI44;
                                    }

                                    int cnt2 = clI4 - clI2 + 1;
                                    if (clI2 > clI4)
                                        cnt2 = (sPPP.Count - clI2) + clI4;

                                    // compute newMappings by pathLength instead of PointCount
                                    double pl1 = 0;
                                    double pl2 = 0;
                                    int plG = 0;

                                    if (clI2 < clI4)
                                    {
                                        pl1 = CalcPathLength(clI2, clI4, sPPP.ToArray(), false);
                                        plG = System.Convert.ToInt32(Math.Ceiling(pl1));
                                    }
                                    else
                                    {
                                        pl1 = CalcPathLength(0, clI4, sPPP.ToArray(), false);
                                        pl2 = CalcPathLength(sPPP.Count - (sPPP.Count - clI2), sPPP.Count - 1, sPPP.ToArray(), true);
                                        plG = System.Convert.ToInt32(Math.Ceiling(pl1 + pl2));
                                    }

                                    double[] newMappings = new double[] { };

                                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                                    {
                                        //if (this._useNormalDist)
                                        //    newMappings = RestrictOrProlongatePoints(this._mappings, plG);
                                        //else
                                        {
                                            newMappings = RestrictOrProlongatePoints(this._mappings, plG / 2 + 2);

                                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                            newMappings.CopyTo(tmp, 0);
                                            List<double> lTmp = newMappings.ToList();
                                            lTmp.RemoveAt(lTmp.Count - 1);
                                            lTmp.Reverse();
                                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                                            newMappings = tmp;
                                        }

                                        if (clI2 > clI4)
                                        {
                                            double j = 0;

                                            double cpl = 0;
                                            double incr = dg.SelectedRows[0].Index / (double)clI4;

                                            for (int i = 0; i <= clI4; i++)
                                            {
                                                int indx = System.Convert.ToInt32(Math.Max(Math.Min(j, dg.Rows.Count - 1), 0));

                                                string? s12 = dg.Rows[indx].Cells["x"].Value.ToString();
                                                string? s22 = dg.Rows[indx].Cells["y"].Value.ToString();

                                                if (s12 != null && s22 != null)
                                                {
                                                    float oldPtX = float.Parse(s12);
                                                    float oldPtY = float.Parse(s22);

                                                    double curDist = 0;
                                                    if (i > 0)
                                                    {
                                                        double dx = sPPP[i].X - sPPP[i - 1].X;
                                                        double dy = sPPP[i].Y - sPPP[i - 1].Y;
                                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                                        cpl += curDist;
                                                    }

                                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(cpl + pl2), newMappings.Count() - 1), 0);

                                                    lO.Add(new PointF(System.Convert.ToSingle(oldPtX + (sPPP[i].X - oldPtX) * newMappings[mIndx]), System.Convert.ToSingle(oldPtY + (sPPP[i].Y - oldPtY) * newMappings[mIndx])));
                                                }
                                                j += incr;
                                            }

                                            for (int i = dg.SelectedRows[0].Index + 1; i <= dg.SelectedRows[cnt - 1].Index - 1; i++)
                                            {
                                                lO.Add(lp.PathPoints[i]);
                                                j += 1;
                                            }

                                            incr = (lp.PointCount - dg.SelectedRows[cnt - 1].Index) / (double)(sPPP.Count - clI2);
                                            cpl = 0;

                                            for (int i = clI2; i <= sPPP.Count - 1; i++)
                                            {
                                                int indx = System.Convert.ToInt32(Math.Max(Math.Min(j, dg.Rows.Count - 1), 0));

                                                string? s12 = dg.Rows[indx].Cells["x"].Value.ToString();
                                                string? s22 = dg.Rows[indx].Cells["y"].Value.ToString();

                                                if (s12 != null && s22 != null)
                                                {
                                                    float oldPtX = float.Parse(s12);
                                                    float oldPtY = float.Parse(s22);

                                                    double curDist = 0;
                                                    if (i > 0 && i > clI2)
                                                    {
                                                        double dx = sPPP[i].X - sPPP[i - 1].X;
                                                        double dy = sPPP[i].Y - sPPP[i - 1].Y;
                                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                                        cpl += curDist;
                                                    }

                                                    int mIndx = Math.Max(Math.Min(System.Convert.ToInt32(plG - cpl), newMappings.Count() - 1), 0);

                                                    lO.Add(new PointF(System.Convert.ToSingle(oldPtX + (sPPP[i].X - oldPtX) * newMappings[mIndx]), System.Convert.ToSingle(oldPtY + (sPPP[i].Y - oldPtY) * newMappings[mIndx])));
                                                }
                                                j += incr;
                                            }
                                        }
                                        else
                                        {
                                            double j = 0;
                                            for (int i = 0; i <= dg.SelectedRows[cnt - 1].Index - 1; i++)
                                            {
                                                lO.Add(lp.PathPoints[i]);
                                                j += 1;
                                            }

                                            double cpl = 0;
                                            double incr = cnt / (double)cnt2;

                                            for (int i = clI2; i <= clI4; i++)
                                            {
                                                int indx = System.Convert.ToInt32(Math.Max(Math.Min(j, dg.Rows.Count - 1), 0));

                                                string? s12 = dg.Rows[indx].Cells["x"].Value.ToString();
                                                string? s22 = dg.Rows[indx].Cells["y"].Value.ToString();

                                                if (s12 != null && s22 != null)
                                                {
                                                    float oldPtX = float.Parse(s12);
                                                    float oldPtY = float.Parse(s22);

                                                    double curDist = 0;
                                                    if (i > 0 && i > clI2)
                                                    {
                                                        double dx = sPPP[i].X - sPPP[i - 1].X;
                                                        double dy = sPPP[i].Y - sPPP[i - 1].Y;
                                                        curDist = Math.Sqrt(dx * dx + dy * dy);
                                                        cpl += curDist;
                                                    }

                                                    int mIndx = Math.Min(System.Convert.ToInt32(pl1 - cpl), newMappings.Count() - 1);

                                                    lO.Add(new PointF(System.Convert.ToSingle(oldPtX + ((sPPP[i].X - oldPtX) * newMappings[mIndx])), System.Convert.ToSingle(oldPtY + ((sPPP[i].Y - oldPtY) * newMappings[mIndx]))));
                                                }
                                                j += incr;
                                            }

                                            for (int i = dg.SelectedRows[0].Index + 1; i <= dg.Rows.Count - 1; i++)
                                                lO.Add(lp.PathPoints[i]);
                                        }
                                    }
                                    else if (clI2 > clI4)
                                    {
                                        for (int i = 0; i <= clI4; i++)
                                            lO.Add(sPPP[i]);

                                        for (int i = dg.SelectedRows[0].Index + 1; i <= dg.SelectedRows[cnt - 1].Index - 1; i++)
                                            lO.Add(lp.PathPoints[i]);

                                        for (int i = clI2; i <= sPPP.Count - 1; i++)
                                            lO.Add(sPPP[i]);
                                    }
                                    else
                                    {
                                        for (int i = 0; i <= dg.SelectedRows[cnt - 1].Index - 1; i++)
                                            lO.Add(lp.PathPoints[i]);

                                        for (int i = clI2; i <= clI4; i++)
                                            lO.Add(sPPP[i]);

                                        for (int i = dg.SelectedRows[0].Index + 1; i <= lp.PathPoints.Length - 1; i++)
                                            lO.Add(lp.PathPoints[i]);
                                    }
                                }
                                else
                                {
                                    cnt = sPPP.Count;
                                    double[] newMappings = new double[] { };

                                    if (cnt > 0 && this._mappings != null && this._mappings.Length > 0)
                                    {
                                        //if (this._useNormalDist)
                                        //    newMappings = RestrictOrProlongatePoints(this._mappings, cnt);
                                        //else
                                        {
                                            newMappings = RestrictOrProlongatePoints(this._mappings, cnt / 2 + 2);

                                            double[] tmp = new double[newMappings.Length * 2 - 1 + 1];
                                            newMappings.CopyTo(tmp, 0);
                                            List<double> lTmp = newMappings.ToList();
                                            lTmp.RemoveAt(lTmp.Count - 1);
                                            lTmp.Reverse();
                                            lTmp.CopyTo(tmp, lTmp.Count + 1);

                                            newMappings = tmp;
                                        }
                                        for (int i = 0; i <= sPPP.Count - 1; i++)
                                        {
                                            double v = dg.Rows.Count / (double)sPPP.Count;
                                            int indx = System.Convert.ToInt32(Math.Max(Math.Min(i * v, dg.Rows.Count - 1), 0));

                                            string? s12 = dg.Rows[indx].Cells["x"].Value.ToString();
                                            string? s22 = dg.Rows[indx].Cells["y"].Value.ToString();

                                            if (s12 != null && s22 != null)
                                            {
                                                float oldPtX = float.Parse(s12);
                                                float oldPtY = float.Parse(s22);
                                                lO.Add(new PointF(System.Convert.ToSingle(oldPtX + (sPPP[i].X - oldPtX) * newMappings[i]), System.Convert.ToSingle(oldPtY + (sPPP[i].Y - oldPtY) * newMappings[i])));
                                            }
                                        }
                                    }
                                    else
                                        lO.AddRange(sPPP);
                                }

                                if (lO != null)
                                {
                                    this._batchEdit = true;
                                    this.DataGridView1.Rows.Clear();

                                    if (lO.Count > 0)
                                    {
                                        int i = 0;
                                        foreach (PointF pt in lO)
                                        {
                                            i += 1;
                                            this.DataGridView1.Rows.Add(i * 100, pt.X, pt.Y);
                                        }
                                    }
                                    this._batchEdit = false;

                                    this._newFigures = new List<int>();
                                    this._newFigures.Add(dg.Rows.Count);
                                }
                                this._pointsAmountChanged = true;
                                List<List<PointF>> pts = GetPointsFromDGV(dg);
                                SetupNewPath(pts);

                                // #######################################################################

                                if (this.Timer1.Enabled)
                                    this.Timer1.Stop();
                                this.Timer1.Start();
                            }
                        }
                }
            }
        }

        private double CalcPathLength(int strtInc, int endInc, PointF[] points, bool pathCrossingZero)
        {
            double dist = 0;
            for (int i = strtInc + 1; i <= endInc; i++)
            {
                double x = points[i].X;
                double y = points[i].Y;
                double x2 = points[i - 1].X;
                double y2 = points[i - 1].Y;
                double dx = x - x2;
                double dy = y - y2;
                double dst = Math.Sqrt(dx * dx + dy * dy);
                dist += dst;

                if (pathCrossingZero && endInc == points.Length - 1)
                {
                    if (i == points.Length - 1)
                    {
                        double xf = points[0].X;
                        double yf = points[0].Y;
                        double xf2 = points[i].X;
                        double yf2 = points[i].Y;
                        double dxf = xf - xf2;
                        double dyf = yf - yf2;
                        double dstf = Math.Sqrt(dx * dx + dy * dy);
                        dist += dstf;
                    }
                }
            }
            return dist;
        }

        private List<PointF> GetWidenedPoints(List<List<PointF>> curPath, float amount)
        {
            List<PointF> widenedPoints = new List<PointF>();
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
                if (gp.PointCount > 2)
                {
                    double f = 100.0 / gp.PathPoints.Length;
                    bool rev = false;
                    if ((!CheckUmlauf(gp.PathPoints)))
                        rev = true;

                    if (rev)
                    {
                        for (int i = 0; i <= gp.PathPoints.Length - 1; i++)
                        {
                            PointF pt = GetWidenedPointRev(gp.PathPoints, i, amount);
                            widenedPoints.Add(pt);
                        }
                    }
                    else
                        for (int i = 0; i <= gp.PathPoints.Length - 1; i++)
                        {
                            PointF pt = GetWidenedPoint(gp.PathPoints, i, amount);
                            widenedPoints.Add(pt);
                        }
                }
            }
            return widenedPoints;
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

        private void EnsureCorrectPathLength(GraphicsPath shiftedPathIn, int start, int cnt)
        {
            if (shiftedPathIn.PointCount - cnt != 0)
            {
                // get currentPath
                List<ChainCode> fList = new List<ChainCode>();

                List<List<PointF>> pts = GetPointsFromDGV(this.DataGridView1);
                using (GraphicsPath gPath = new GraphicsPath())
                {
                    for (int i = 0; i <= pts.Count - 1; i++)
                    {
                        gPath.StartFigure();
                        gPath.AddLines(pts[i].ToArray());
                        gPath.CloseFigure();
                    }

                    Bitmap? b = null;
                    if (_bmpBU != null && AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 4L))
                    {
                        b = new Bitmap(this._bmpBU.Width, this._bmpBU.Height);

                        using (Graphics g = Graphics.FromImage(b))
                        {
                            gPath.FillMode = FillMode.Winding;
                            g.FillPath(Brushes.Black, gPath);
                        }

                        // Dim ffff As New Form
                        // ffff.BackgroundImage = b
                        // ffff.ShowDialog()

                        ChainFinder fbmp = new ChainFinder();
                        fList = fbmp.GetOutline(b, 0, false, 0, false, 0, false);

                        if (fList.Count > 0)
                        {
                            // remove 1px chains
                            for (int i = fList.Count - 1; i >= 0; i += -1)
                            {
                                if (fList[i].Coord.Count <= 4)
                                    fList.RemoveAt(i);
                            }

                            fList = fList.OrderBy(a => a.Coord.Count).ToList();
                            fList.Reverse();
                        }
                    }
                    else
                        MessageBox.Show("Not emnough Memory");

                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }
                }

                int l = this.DataGridView1.SelectedRows.Cast<DataGridViewRow>().Count();

                if (l == 0)
                    l = this.DataGridView1.Rows.Count;

                int f = this.DataGridView1.SelectedRows[this.DataGridView1.SelectedRows.Cast<DataGridViewRow>().Count() - 1].Index;

                //for (int i = f; i <= l - 1; i++)
                //{
                //    if ((this.DataGridView1.Rows[i].Cells["x"].Value != null) && (this.DataGridView1.Rows[i].Cells["y"].Value != null))
                //    {
                //        float oldPtX = float.Parse(this.DataGridView1.Rows[i].Cells["x"].Value.ToString());
                //        float oldPtY = float.Parse(this.DataGridView1.Rows[i].Cells["y"].Value.ToString());

                //        bool found = false;
                //        int z = -1;
                //        for (int j = 0; j <= fList.Count - 1; j++)
                //            List<Point> p = fList[i].Coord.ToList();
                //    }
                //}
            }
        }

        private GraphicsPath GetPath4(bool reducePathPoints)
        {
            DataGridView dg = this.DataGridView1;
            List<List<PointF>> pts = GetPointsFromDGV(dg);

            SetupNewPath(pts);

            GraphicsPath fP = new GraphicsPath();

            if (CheckShift())
            {
                MorphologicalOperations m2 = new MorphologicalOperations();
                GraphicsPathData? gPath = m2.ShiftCoords4(_fPath, (float)this.numericUpDown2.Value, false, false, this._shiftFractionMode);
                GraphicsPath gPath4 = new GraphicsPath();

                if (gPath != null && gPath.AllPoints != null && gPath.AllPoints.Count > 0)
                {
                    this._newFigures = new List<int>();
                    for (int cnt = 0; cnt <= gPath.AllPoints.Count - 1; cnt++) // loop over each subpath
                    {
                        List<PointF> lList = gPath.AllPoints[cnt];

                        using (GraphicsPath gPath2 = new GraphicsPath()) // path for current subpath
                        {
                            gPath2.AddLines(lList.ToArray());

                            if (reducePathPoints)
                            {
                                // approxlines etc
                                List<Point> fList = gPath2.PathPoints.AsEnumerable().ToList().ConvertAll(aa => new Point(System.Convert.ToInt32(aa.X), System.Convert.ToInt32(aa.Y)));
                                double epsilon = Math.Sqrt(2.0) / 2.0;
                                ChainFinder fbmp = new ChainFinder();
                                fList = fbmp.RemoveColinearity(fList, true, 4);
                                // approx lines
                                fList = fbmp.ApproximateLines(fList, epsilon);
                                gPath2.Reset();
                                gPath2.AddLines(fList.ToArray());
                            }

                            if (gPath2.PointCount > 1)
                            {
                                gPath2.FillMode = FillMode.Alternate;
                                gPath4.AddPath(gPath2, false); // add to large (shifted) path
                                this._newFigures.Add(gPath4.PointCount);
                            }
                        }
                    }

                    if (gPath4 != null && gPath4.PointCount > 1)
                    {
                        if (FPath != null)
                        {
                            FPath.Dispose();
                            FPath = null;
                        }

                        FPath = gPath4; // set as reult
                    }
                }

                gPath = null;

                if (gPath4 != null && gPath4.PointCount > 1)
                    fP.AddPath(gPath4, false);
            }

            return fP;
        }

        private GraphicsPath GetPath(bool reducePathPoints)
        {
            DataGridView dg = this.DataGridView1;
            List<List<PointF>> pts = GetPointsFromDGV(dg);

            SetupNewPath(pts);

            GraphicsPath fP = new GraphicsPath();

            if (CheckShift())
            {
                ChainFinder c2 = new ChainFinder();

                GraphicsPathData? gPath = c2.ShiftCoords(_fPath, (float)this.numericUpDown2.Value, false, false, this._shiftFractionMode);
                GraphicsPath gPath4 = new GraphicsPath();

                if (gPath != null && gPath.AllPoints != null && gPath.AllPoints.Count > 0)
                {
                    this._newFigures = new List<int>();
                    for (int cnt = 0; cnt <= gPath.AllPoints.Count - 1; cnt++) // loop over each subpath
                    {
                        List<PointF> lList = gPath.AllPoints[cnt];

                        using (GraphicsPath gPath2 = new GraphicsPath()) // path for current subpath
                        {
                            gPath2.AddLines(lList.ToArray());

                            // approxlines etc
                            if (reducePathPoints)
                            {
                                List<Point> fList = gPath2.PathPoints.AsEnumerable().ToList().ConvertAll(aa => new Point(System.Convert.ToInt32(aa.X), System.Convert.ToInt32(aa.Y)));
                                double epsilon = Math.Sqrt(2.0) / 2.0;
                                ChainFinder fbmp = new ChainFinder();
                                fList = fbmp.RemoveColinearity(fList, true, 4);
                                // approx lines
                                fList = fbmp.ApproximateLines(fList, epsilon);
                                gPath2.Reset();
                                gPath2.AddLines(fList.ToArray());
                            }

                            if (gPath2.PointCount > 1)
                            {
                                gPath2.FillMode = FillMode.Alternate;
                                gPath4.AddPath(gPath2, false); // add to large (shifted) path
                                this._newFigures.Add(gPath4.PointCount);
                            }
                        }
                    }

                    if (gPath4 != null && gPath4.PointCount > 1)
                    {
                        if (FPath != null)
                        {
                            FPath.Dispose();
                            FPath = null;
                        }

                        FPath = gPath4; // set as reult
                    }
                }

                gPath = null;

                if (gPath4 != null && gPath4.PointCount > 1)
                    fP.AddPath(gPath4, false);
            }

            return fP;
        }

        // Private Function GetPathF() As GraphicsPath
        // Dim dg As DataGridView = Me.DataGridView1
        // Dim pts As List(Of List(Of PointF)) = GetPointsFromDGV(dg)

        // SetupNewPath(pts)

        // Dim fP As New GraphicsPath

        // If CheckShift() Then
        // Dim c2 As New ChainFinder()
        // Dim gPath As GraphicsPathData = c2.GetNewSpacedPath(_fPath, False, Me._shiftFractionMode)
        // Dim gPath4 As New GraphicsPath()

        // If Not gPath Is Nothing AndAlso Not gPath.AllPoints Is Nothing AndAlso gPath.AllPoints.Count > 0 Then
        // For cnt As Integer = 0 To gPath.AllPoints.Count - 1 'loop over each subpath
        // Dim lList As List(Of PointF) = gPath.AllPoints(cnt)

        // Using gPath2 As New GraphicsPath 'path for current subpath
        // gPath2.AddLines(lList.ToArray())

        // If gPath2.PointCount > 1 Then
        // gPath2.FillMode = FillMode.Alternate
        // gPath4.AddPath(gPath2, False) 'add to large (shifted) path
        // End If
        // End Using
        // Next

        // If Not gPath4 Is Nothing AndAlso gPath4.PointCount > 1 Then
        // If Not FPath Is Nothing Then
        // FPath.Dispose()
        // FPath = Nothing
        // End If

        // FPath = gPath4 'set as reult
        // End If
        // End If

        // gPath = Nothing

        // If Not gPath4 Is Nothing AndAlso gPath4.PointCount > 1 Then
        // fP.AddPath(gPath4, False)
        // End If
        // End If

        // Return fP
        // End Function

        private GraphicsPath GetPath2(bool reducePathPoints)
        {
            DataGridView dg = this.DataGridView1;
            List<List<PointF>> pts = GetPointsFromDGV(dg);

            SetupNewPath(pts);

            GraphicsPath fP = new GraphicsPath();

            if (CheckShift())
            {
                ChainFinder c2 = new ChainFinder();
                GraphicsPath? gPath = c2.ShiftCoords2(_fPath, (float)this.numericUpDown2.Value, false, this._shiftFractionMode);
                GraphicsPath gPath4 = new GraphicsPath();

                if (gPath != null && gPath.PointCount > 1)
                {
                    this._newFigures = new List<int>();
                    List<List<PointF>> lL = c2.SplitPath(gPath);

                    for (int cnt = 0; cnt <= lL.Count - 1; cnt++) // loop over each subpath
                    {
                        List<PointF> lList = lL[cnt];

                        using (GraphicsPath gPath2 = new GraphicsPath()) // path for current subpath
                        {
                            gPath2.AddLines(lList.ToArray());

                            if (reducePathPoints)
                            {
                                // approxlines etc
                                List<Point> fList = gPath2.PathPoints.AsEnumerable().ToList().ConvertAll(aa => new Point(System.Convert.ToInt32(aa.X), System.Convert.ToInt32(aa.Y)));
                                double epsilon = Math.Sqrt(2.0) / 2.0;
                                ChainFinder fbmp = new ChainFinder();
                                fList = fbmp.RemoveColinearity(fList, true, 4);
                                // approx lines
                                fList = fbmp.ApproximateLines(fList, epsilon);
                                gPath2.Reset();
                                gPath2.AddLines(fList.ToArray());
                            }

                            if (gPath2.PointCount > 1)
                            {
                                gPath2.FillMode = FillMode.Alternate;
                                gPath4.AddPath(gPath2, false); // add to large (shifted) path
                                this._newFigures.Add(gPath4.PointCount);
                            }
                        }
                    }

                    if (gPath4 != null && gPath4.PointCount > 1)
                    {
                        if (FPath != null)
                        {
                            FPath.Dispose();
                            FPath = null;
                        }

                        FPath = gPath4; // set as reult
                    }
                }
                else
                    MessageBox.Show("Point shift unsuccessful. Maybe the amount \"shift\" is too big.");

                // gPath.Dispose()
                gPath = null;

                if (gPath4 != null && gPath4.PointCount > 1)
                    fP.AddPath(gPath4, false);
            }

            return fP;
        }

        private bool CheckShift()
        {
            return this.numericUpDown2.Value != 0;
        }

        private void Button13_Click(object sender, EventArgs e)
        {

        }

        private void RestoreSelection()
        {
            CheckAndSelect();
        }
    }
}
