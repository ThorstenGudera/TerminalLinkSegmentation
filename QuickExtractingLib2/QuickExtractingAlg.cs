using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.ComponentModel;

namespace QuickExtractingLib2
{
    //roughly following:
    //Interactive Segmentation with Intelligent Scissors
    //Eric N.Mortensen
    //William A.Barrett#
    //https://courses.cs.washington.edu/courses/cse455/09wi/readings/seg_scissors.pdf
    public class QuickExtractingAlg : IDisposable
    {
        public double valL { get; set; }
        public double valM { get; set; }
        public double valG { get; set; }
        public double valP { get; set; }
        public double valI { get; set; }
        public double valO { get; set; }
        public double valCl { get; set; }
        public double valCol { get; set; }
        public int laplTh { get; set; }
        public double edgeWeight { get; set; }
        public bool doR { get; set; }
        public bool doG { get; set; }
        public bool doB { get; set; }

        private int neighborsCount;

        public List<PointF>? SeedPoints { get; set; }
        public List<List<PointF>>? CurPath { get; set; }
        public PicAlgLDG? picAlg { get; set; }
        public int MaxIterations { get; private set; }
        public System.Collections.BitArray? PP { get; private set; }
        public List<QEData>? SortedPixelList { get; private set; }
        public List<double>? EdgeGrades { get; private set; }
        public List<int>? BackPointers { get; private set; }
        public System.Collections.BitArray? Edges { get; private set; }

        private int _stride;
        private BitmapData? _bmpData;
        private int _zaehl;
        private int _minIndx;
        private double _minValue;

        public List<Point>? Path { get; private set; }
        public bool CancelFlag { get; set; }
        public bool AllScanned { get; set; }
        public int dist { get; set; }
        public Point PreviouslyInsertedPoint { get; set; }
        public BitmapData? bmpDataForValueComputation { get; set; }
        public bool MouseClicked { get; set; }
        public List<PointF>? TempPath { get; set; }
        public bool isRunning { get; private set; }
        public List<Point>? TempData { get; private set; }
        public List<int>? Neighbors { get; private set; }

        private double Scale = 1.0;
        public Bitmap? bmp { get; private set; }
        private Bitmap? bmpForValueComputation;
        public BackgroundWorker bgw { get; set; }
        public int NotifyEach { get; set; }

        public event EventHandler<MatchEventArgs>? NotifyMatch;
        public event EventHandler<NotifyEventArgs>? NotifyEdges;

        private object _lockObject = new object();

        public QuickExtractingAlg(Bitmap? bmp, Bitmap? bmpForValueComputation, Point pt,
                bool doR, bool doG, bool doB, bool doScale, int neighbors, System.ComponentModel.BackgroundWorker bgw)
        {
            this.bmp = bmp;
            this.bmpForValueComputation = bmpForValueComputation;

            this.bgw = bgw;

            this.NotifyEach = 10000;

            this.valL = 0.3;
            this.valM = 0.3;
            this.valG = 0.1;

            this.valP = 0.1;
            this.valI = 0.1;
            this.valO = 0.1;

            this.valCl = 0.1;
            this.valCol = 0.1;

            this.laplTh = 184;
            this.edgeWeight = 0.5;

            this.doR = doR;
            this.doG = doG;
            this.doB = doB;

            this.neighborsCount = neighbors;

            this.SeedPoints = new List<PointF>();
            this.CurPath = new List<List<PointF>>();  //list to return to main thread

            if (bmp != null && bmpForValueComputation != null)
            {
                this._bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                this.bmpDataForValueComputation = bmpForValueComputation.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                this.picAlg = new PicAlgLDG(bmpDataForValueComputation);

                var sz = this._bmpData.Width * this._bmpData.Height;

                this.MaxIterations = sz;
                this._stride = this._bmpData.Stride;

                this.InitNeighbors(this._stride, this.neighborsCount);

                this.PP = new System.Collections.BitArray(sz, false);
                this.SortedPixelList = new List<QEData>();
                Stack<int> addresses = new Stack<int>();
                addresses.Push(pt.Y * this._stride + pt.X * 4);
                this.SortedPixelList.Add(new QEData(0, addresses));
                this.EdgeGrades = new List<double>();
                for (int i = 0; i < sz; i++)
                    this.EdgeGrades.Add(double.MaxValue);
                this.BackPointers = new List<int>();
                for (int i = 0; i < sz; i++)
                    this.BackPointers.Add(-1);
                this.Edges = new System.Collections.BitArray(sz, false);

                this._zaehl = 0;

                this._minIndx = 0;
                this._minValue = 0.0;

                this.Path = new List<Point>();

                this.CancelFlag = false;
                this.AllScanned = false;

                this.Scale = 1;

                if (doScale)
                {
                    double d = (this.valL + this.valM + this.valG);
                    if (d < 1.0 && d > 0.0)
                        this.Scale = 1.0 / d;
                    else
                        this.Scale = 1.0;
                }

                this.dist = 1;

                this.PreviouslyInsertedPoint = new Point(-1, -1);

                byte[] f = this.picAlg.InitBmpDataForValueComputation(this._bmpData, this._stride, this.doR, this.doG, this.doB);

                unsafe
                {
                    byte* p = (byte*)bmpDataForValueComputation.Scan0;

                    //for (int i = 0; i < f.Length; i++)
                    Parallel.For(0, f.Length, i =>
                    {
                        p[i] = f[i];
                    });

                    this.bmpDataForValueComputation = bmpDataForValueComputation;
                }

                this.MouseClicked = true;
                this.TempPath = new List<PointF>();
                this.isRunning = false;
                this.TempData = new List<Point>();
            }
        }

        public void ReInit(int x, int y, bool doScale, int neighbors)
        {
            this._zaehl = 1;

            this.isRunning = false;

            if (this._bmpData != null)
            {
                var sz = this._bmpData.Width * this._bmpData.Height;

                lock (this._lockObject)
                {
                    this.PP = new System.Collections.BitArray(sz, false);
                    this.SortedPixelList = new List<QEData>();
                    Stack<int> addresses = new Stack<int>();
                    addresses.Push(y * this._stride + x * 4);
                    this.SortedPixelList.Add(new QEData(0, addresses));
                    this.EdgeGrades = new List<double>();
                    for (int i = 0; i < sz; i++)
                        this.EdgeGrades.Add(double.MaxValue);
                    this.BackPointers = new List<int>();
                    for (int i = 0; i < sz; i++)
                        this.BackPointers.Add(-1);
                    this.Edges = new System.Collections.BitArray(sz, false);
                }
                this._minIndx = 0;
                this._minValue = 0;

                this.CancelFlag = false;
                this.AllScanned = false;

                this.Scale = 1;

                if (doScale)
                {
                    double d = (this.valL + this.valM + this.valG);
                    if (d < 1.0 && d > 0.0)
                        this.Scale = 1.0 / d;
                    else
                        this.Scale = 1.0;
                }

                this.neighborsCount = neighbors;
                this.InitNeighbors(this._stride, this.neighborsCount);
            }
        }

        public void InitCostMapStandardCalc()
        {
            if (this.picAlg == null)
                throw new Exception("PicAlg is not defined");

            this.picAlg.Mcc = new MagnitudeAndPixelCostCalculatorStandard();
            this.picAlg.CostMaps = this.picAlg.Mcc.CalculateRamps();
        }

        public void DisposeBmpData()
        {
            try
            {
                if (bmp != null && this._bmpData != null)
                    bmp.UnlockBits(this._bmpData);
            }
            catch
            {

            }

            try
            {
                if (bmpForValueComputation != null && this.bmpDataForValueComputation != null)
                    bmpForValueComputation.UnlockBits(this.bmpDataForValueComputation);
            }
            catch
            {

            }

            try
            {
                if (bmp != null)
                    this.bmp.Dispose();
                this.bmp = null;
            }
            catch
            {

            }

            try
            {
                if (bmpForValueComputation != null)
                    this.bmpForValueComputation.Dispose();
                this.bmpForValueComputation = null;
            }
            catch
            {

            }
        }

        public void PrepareForTranslatedPaths(Bitmap? bmp, Bitmap? bmpForValueComputation, int transX, int transY)
        {
            DisposeBmpData();

            this.bmp = bmp;

            if (bmp != null && bmpForValueComputation != null && this.picAlg != null)
            {
                this.bmpForValueComputation = bmpForValueComputation;
                this._bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                this.bmpDataForValueComputation = bmpForValueComputation.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                var sz = _bmpData.Width * _bmpData.Height;

                this.MaxIterations = sz;
                this._stride = _bmpData.Stride;

                this.picAlg.bmpDataForValueComputation = bmpDataForValueComputation;

                byte[] f = this.picAlg.InitBmpDataForValueComputation(this._bmpData, this._stride, this.doR, this.doG, this.doB);

                unsafe
                {
                    byte* p = (byte*)bmpDataForValueComputation.Scan0;

                    //for (int i = 0; i < f.Length; i++)
                    Parallel.For(0, f.Length, i =>
                    {
                        p[i] = f[i];
                    });

                    this.bmpDataForValueComputation = bmpDataForValueComputation;
                }

                this.PreviouslyInsertedPoint = new Point(this.PreviouslyInsertedPoint.X + transX,
                    this.PreviouslyInsertedPoint.Y + transY);

                if (this.picAlg.Mcc != null)
                {
                    if (this.CurPath != null && this.CurPath.Count > 0)
                    {
                        var l = this.CurPath[this.CurPath.Count - 1];
                        this.picAlg.Mcc.AddressesGray = l.ConvertAll((a) => { return (int)a.X + (int)a.Y * this._bmpData.Width; });
                    }

                    this.picAlg.Mcc.DataGray = this.picAlg.GrayRepresentation;
                    this.picAlg.Mcc.BmpDataForValueComputation = bmpDataForValueComputation;
                    this.picAlg.Mcc.Stride = this._stride;
                }
            }
        }

        private Tuple<double, bool>? ComputeValue(int address, int neighbor, bool procNeighbor)
        {
            if (this.picAlg != null && this.bmpDataForValueComputation != null)
                return this.picAlg.ComputeValueToNeighbor(this.bmpDataForValueComputation, address,
                    neighbor, this._stride, this.valL, this.valM, this.valG, this.laplTh, procNeighbor,
                    this.dist, this.edgeWeight, this.Scale, false, this.valP, this.valI, this.valO,
                    this.valCl, this.valCol);
            return null;
        }

        public void EnumeratePaths(bool notify)
        {
            this._minIndx = -1;
            this.TempPath?.Clear();
            this.TempData?.Clear();

            if (!this.isRunning && this._bmpData != null)
            {
                this.isRunning = true;

                this.SortedPixelList?.Sort((a, b) => a.Weight - b.Weight);

                while (this.SortedPixelList?.Count > 0 && this._zaehl < this.MaxIterations && !this.CancelFlag)
                {
                    if (this.CancelFlag || this.bgw.CancellationPending)
                        break;

                    int z1 = this._minIndx;

                    if (z1 == -1)
                    {
                        this._minIndx = 0;
                        this._minValue = this.SortedPixelList[0].Weight;
                    }

                    //var l = this.SortedPixelList[0].list;
                    Stack<int>? l = this.SortedPixelList[this._minIndx].Adresses;
                    if (l != null && this.SeedPoints != null && this.CurPath != null)
                    {
                        int z = l.Pop();

                        if (l.Count == 0)
                            this.SortedPixelList.RemoveAt(0);

                        this._minIndx = -1;

                        this.DoStep(z);

                        if (notify)
                        {
                            if (this._zaehl % this.NotifyEach == 0)
                            {
                                NotifyEventArgs ne = new NotifyEventArgs() { msg = "tempData", TempData = this.TempData };
                                if (this.NotifyEdges != null)
                                    NotifyEdges(this, ne);
                                this.TempData?.Clear();
                            }
                        }

                        this._zaehl += 1;

                        if (z == this.PreviouslyInsertedPoint.X * 4 + this.PreviouslyInsertedPoint.Y * this._stride)
                        {
                            List<PointF>? path = this.GetPath(this.PreviouslyInsertedPoint.X, this.PreviouslyInsertedPoint.Y);

                            if (this.MouseClicked)
                            {
                                if (path?.Count > 0)
                                {
                                    this.SeedPoints[this.SeedPoints.Count - 1] = new PointF(path[path.Count - 1].X, path[path.Count - 1].Y);
                                    this.SeedPoints[this.SeedPoints.Count - 2] = new PointF(path[0].X, path[0].Y);
                                    this.CurPath.Add(path);
                                }
                            }
                            else if (path != null)
                                this.TempPath = path;

                            break;
                        }
                    }
                }

                if (this.SortedPixelList?.Count == 0 && !this.CancelFlag)
                    this.AllScanned = true;

                this.isRunning = false;
            }
        }

        public void EnumeratePaths(bool notify, string? msg)
        {
            this._minIndx = -1;
            this.TempPath?.Clear();
            this.TempData?.Clear();

            if (!this.isRunning && this._bmpData != null)
            {
                this.isRunning = true;
                int found = -1;

                this.SortedPixelList?.Sort((a, b) => a.Weight - b.Weight);

                while (this.SortedPixelList?.Count > 0 && this._zaehl < this.MaxIterations && !this.CancelFlag)
                {
                    if (this.CancelFlag || this.bgw.CancellationPending)
                        break;

                    int z1 = this._minIndx;

                    if (z1 == -1)
                    {
                        this._minIndx = 0;
                        this._minValue = this.SortedPixelList[0].Weight;
                    }

                    //var l = this.SortedPixelList[0].list;
                    Stack<int>? l = this.SortedPixelList[this._minIndx].Adresses;
                    if (l != null)
                    {
                        int z = l.Pop();

                        if (l.Count == 0)
                            this.SortedPixelList.RemoveAt(0);

                        this._minIndx = -1;

                        this.DoStep(z);

                        if (notify)
                        {
                            if (this._zaehl % this.NotifyEach == 0)
                            {
                                NotifyEventArgs ne = new NotifyEventArgs() { msg = "tempData", TempData = this.TempData };
                                if (this.NotifyEdges != null)
                                    NotifyEdges(this, ne);
                                this.TempData?.Clear();
                            }
                        }

                        this._zaehl += 1;

                        int aaa = this.PreviouslyInsertedPoint.X + this.PreviouslyInsertedPoint.Y * this._bmpData.Width;
                        if (!this.MouseClicked && found != aaa && this.BackPointers != null)
                        {
                            int zzz = this.BackPointers[aaa];

                            if (zzz != -1)
                            {
                                List<PointF>? path = this.GetPath(this.PreviouslyInsertedPoint.X, this.PreviouslyInsertedPoint.Y);
                                path?.Reverse();

                                found = aaa;

                                if (path != null)
                                {
                                    this.TempPath = path;

                                    MatchEventArgs me = new MatchEventArgs()
                                    {
                                        CurPath = this.CurPath,
                                        msg = msg,
                                        continueWork = (this.MouseClicked) ? false : true,
                                        SeedPoints = this.SeedPoints,
                                        TempPath = this.TempPath,
                                        Ramps = (this.picAlg?.CostMaps != null) ? this.picAlg.CostMaps.Ramps : null
                                    };
                                    if (this.NotifyMatch != null)
                                        NotifyMatch(this, me);
                                }
                                //break;
                            }
                        }


                        if (z == this.PreviouslyInsertedPoint.X * 4 + this.PreviouslyInsertedPoint.Y * this._stride)
                        {
                            List<PointF>? path = this.GetPath(this.PreviouslyInsertedPoint.X, this.PreviouslyInsertedPoint.Y);
                            path?.Reverse();

                            if (this.MouseClicked && path != null)
                            {
                                this.CurPath?.Add(path);
                                this.SeedPoints?.Add(new Point(this.PreviouslyInsertedPoint.X, this.PreviouslyInsertedPoint.Y));

                                MatchEventArgs me = new MatchEventArgs()
                                {
                                    CurPath = this.CurPath,
                                    msg = msg,
                                    continueWork = false,
                                    SeedPoints = this.SeedPoints,
                                    TempPath = this.TempPath,
                                    Ramps = (this.picAlg?.CostMaps != null) ? this.picAlg.CostMaps.Ramps : null
                                };
                                if (NotifyMatch != null)
                                    NotifyMatch(this, me);
                                break;
                            }
                            else if (path != null)
                            {
                                this.TempPath = path;

                                if (!this.CancelFlag)
                                {
                                    MatchEventArgs me = new MatchEventArgs()
                                    {
                                        CurPath = this.CurPath,
                                        msg = msg,
                                        continueWork = true,
                                        SeedPoints = this.SeedPoints,
                                        TempPath = this.TempPath,
                                        Ramps = (this.picAlg?.CostMaps != null) ? this.picAlg.CostMaps.Ramps : null
                                    };
                                    if (NotifyMatch != null)
                                        NotifyMatch(this, me);
                                }
                            }

                            //break;
                        }
                    }
                }

                if ((this.SortedPixelList?.Count == 0 && !this.CancelFlag) || this._zaehl >= this.MaxIterations)
                    this.AllScanned = true;

                this.isRunning = false;
            }
        }

        private void DoStep(int address)
        {
            bool isEdge = false;

            if (this.Neighbors != null)
                for (int i = 0; i < this.Neighbors.Count; i++)
                {
                    if (this.CancelFlag || this.bgw.CancellationPending)
                        break;

                    int z = (address + this.Neighbors[i]);
                    if (this._bmpData != null && (z >= 0) && (z < (this._stride * this._bmpData.Height)))
                    {
                        int zz = (int)(z / 4.0);
                        int aa = (int)(address / 4.0);
                        if (this.PP?.Count > aa)
                            this.PP[aa] = true;

                        if (this.PP?[zz] == false)
                        {
                            int l = this.Neighbors[i];
                            if ((l == 4) && (address % this._stride != (this._stride - 4)) ||
                                    (l == -4) && (address % this._stride != 0) ||
                                    (l == -this._stride) && (address >= this._stride) ||
                                    (l == this._stride) && (address < this._stride * (this._bmpData.Height - 1)) ||
                                    (l == this._stride + 4) && (address < this._stride * (this._bmpData.Height - 1)) && (address % this._stride != (this._stride - 4)) ||
                                    (l == this._stride - 4) && (address < this._stride * (this._bmpData.Height - 1)) && (address % this._stride != 0) ||
                                    (l == -this._stride + 4) && (address >= this._stride) && (address % this._stride != (this._stride - 4)) ||
                                    (l == -this._stride - 4) && (address >= this._stride) && (address % this._stride != 0))
                            {

                                int indx = i;
                                Tuple<double, bool>? t = this.ComputeValue(address, z, (i & 0x01) == 1 /* true */);
                                if (t != null)
                                {
                                    if (double.IsNaN(t.Item1))
                                        break;

                                    double tmp = t.Item1;

                                    if (!isEdge && t.Item2 == true)
                                        isEdge = true;

                                    if (!((indx & 0x01) == 1) && this.neighborsCount == 1)
                                        tmp *= Math.Sqrt(2.0);

                                    double tmp2 = tmp;
                                    tmp += this._minValue;

                                    bool l2 = false;
                                    int jjj = Int32.MaxValue;
                                    int indx2 = -1;

                                    for (int jj = this._minIndx + 1; jj < this.SortedPixelList?.Count; jj++)
                                    {
                                        Stack<int>? c = this.SortedPixelList[jj].Adresses;
                                        if (c != null)
                                        {
                                            bool ic = c.Contains(z);

                                            if (ic)
                                            {
                                                l2 = true;
                                                jjj = this.SortedPixelList[jj].Weight;
                                                indx2 = jj;
                                                break;
                                            }
                                        }
                                    }

                                    int tmp4 = (int)Math.Round(tmp);

                                    if (l2 && tmp4 < jjj)
                                    {
                                        this.SortedPixelList?[indx2]?.Adresses?.Push(z);

                                        if (tmp4 < this._minValue)
                                        {
                                            this._minIndx = indx2;
                                            this._minValue = tmp4;
                                        }

                                        if (indx2 < this._minIndx)
                                            this._minIndx = indx2;

                                        if (zz < this.BackPointers?.Count)
                                            this.BackPointers[zz] = address;
                                    }

                                    if (!l2 && this.SortedPixelList != null)
                                    {
                                        int aaa = tmp4;
                                        int indx4 = this.SortedPixelList.FindIndex((a) => a.Weight == aaa);

                                        if (indx4 == -1)
                                        {
                                            Stack<int> ll = new Stack<int>();
                                            ll.Push(z);

                                            //int pos = 0;
                                            //while ((pos < this.SortedPixelList.Count) && (aaa > this.SortedPixelList[pos].Weight))
                                            //    pos++;

                                            //this.SortedPixelList.Insert(pos, new QEData(aaa, ll));

                                            this.SortedPixelList.Add(new QEData(aaa, ll));
                                            if (this.SortedPixelList.Count > 1)
                                                this.SortedPixelList.Sort((a, b) => a.Weight - b.Weight);
                                        }
                                        else
                                            this.SortedPixelList[indx4]?.Adresses?.Push(z);

                                        if (tmp4 < this._minValue)
                                        {
                                            this._minValue = tmp4;
                                        }

                                        if (indx4 < this._minIndx)
                                            this._minIndx = indx4;

                                        if (zz < this.BackPointers?.Count)
                                            this.BackPointers[zz] = address;
                                    }

                                    if (zz < this.EdgeGrades?.Count && tmp2 < this.EdgeGrades[zz])
                                        this.EdgeGrades[zz] = tmp2 / 3.0;
                                }
                            }
                        }
                    }
                }

            if (isEdge && this.Edges != null && this.TempData != null)
            {
                this.Edges[(int)(address / 4.0)] = isEdge;
                this.TempData.Add(new Point(address % this._stride / 4, (int)(address / this._stride)));
            }
        }

        private void InitNeighbors(int stride, int neighborsCount)
        {
            this.Neighbors = new List<int>();
            // clockwise
            if (neighborsCount == 1)
            {
                this.Neighbors.Add(this._stride * -1 + 4);
                this.Neighbors.Add(4);
                this.Neighbors.Add(this._stride + 4);
                this.Neighbors.Add(this._stride);
                this.Neighbors.Add(this._stride - 4);
                this.Neighbors.Add(-4);
                this.Neighbors.Add(this._stride * -1 - 4);
                this.Neighbors.Add(this._stride * -1);
            }
            else
            {
                this.Neighbors.Add(4);
                this.Neighbors.Add(this._stride);
                this.Neighbors.Add(-4);
                this.Neighbors.Add(this._stride * -1);
            }
        }

        // get the results as a list holding the coords of the current segment
        public List<PointF>? GetPath(int x, int y)
        {
            if (this._bmpData != null && this.Edges != null)
            {
                var lOut = new List<PointF>();
                var zz = y * this._bmpData.Width + x;
                if (this.Edges[zz])
                    lOut.Add(new PointF(x, y));

                if (this.BackPointers != null)
                    while (this.BackPointers[zz] > -1)
                    {
                        zz = (int)(this.BackPointers[zz] / 4.0);
                        if (this.Edges[zz])
                            lOut.Add(new PointF(zz % this._bmpData.Width, (int)(zz / this._bmpData.Width)));
                    }

                return lOut;
            }
            return null;
        }

        public void Dispose()
        {
            DisposeBmpData();
        }

        public void SetVariables(double v1, int v2, double v3, double v4, double v5, double v6, double v7,
            double v8, double v9, double v10)
        {
            this.valL = v1;
            this.laplTh = v2;
            this.valM = v3;
            this.valG = v4;
            this.edgeWeight = v5;

            this.valP = v6;
            this.valI = v7;
            this.valO = v8;

            this.valCl = v9;
            this.valCol = v10;
        }

        public void SetScaleValues(bool doScale)
        {
            this.Scale = 1;

            if (doScale)
            {
                double d = (this.valL + this.valM + this.valG);
                if (d < 1.0 && d > 0.0)
                    this.Scale = 1.0 / d;
                else
                    this.Scale = 1.0;
            }
        }

        public void SetColorChannels(bool doR, bool doG, bool doB)
        {
            if (this.picAlg != null && this._bmpData != null)
            {
                this.doR = doR;
                this.doG = doG;
                this.doB = doB;

                this.picAlg.Dispose();
                this.picAlg = new PicAlgLDG(bmpDataForValueComputation);

                byte[] f = this.picAlg.InitBmpDataForValueComputation(this._bmpData, this._stride, this.doR, this.doG, this.doB);

                if (bmpDataForValueComputation != null)
                    unsafe
                    {
                        byte* p = (byte*)bmpDataForValueComputation.Scan0;

                        //for (int i = 0; i < f.Length; i++)
                        Parallel.For(0, f.Length, i =>
                        {
                            p[i] = f[i];
                        });

                        this.bmpDataForValueComputation = bmpDataForValueComputation;
                    }
            }
        }

        public void SetNeighbors(int mode)
        {
            this.neighborsCount = mode;
            this.InitNeighbors(this._stride, mode);
        }

        public void SetColorBmp(bool useColVals)
        {
            if (this.picAlg != null)
            {
                this.picAlg.BmpDataColor = this._bmpData;
                this.picAlg.UseColVal = useColVals;
            }
        }
    }
}
