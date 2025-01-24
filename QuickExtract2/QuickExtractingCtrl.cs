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
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace QuickExtract2
{
    //I'll move some more methods from frmQuickExtract to this Control time after time...
    public partial class QuickExtractingCtrl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public QuickExtractingAlg? Alg { get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public  List<double[]>? Ramps { get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<PointF>? SeedPoints { get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<PointF>? TempPath {  get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<List<PointF>>? CurPath {  get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<List<PointF>>? EditedPath {  get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<List<List<PointF>>>? PathList { get; set; } = null;

        public QuickExtractingCtrl()
        {
            InitializeComponent();

            this.CurPath = new List<List<PointF>>();
            this.TempPath = new List<PointF>();
            this.SeedPoints = new List<PointF>();
        }

        internal void AddTempPath(bool autoSeedPoints, bool addSeedPoint, int ix, int iy)
        {
            if (this.TempPath?.Count > 0)
            {
                double minDist = Int32.MaxValue;
                int minIndx = -1;
                for (int i = 0; i <= this.TempPath.Count - 1; i++)
                {
                    double dx = ix - this.TempPath[i].X;
                    double dy = iy - this.TempPath[i].Y;

                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist < minDist)
                    {
                        minIndx = i;
                        minDist = dist;
                    }
                }

                if (minIndx != -1)
                {
                    if (autoSeedPoints && this.SeedPoints != null)
                    {
                        if (addSeedPoint)
                            this.SeedPoints.Add(new PointF(this.TempPath[minIndx].X, this.TempPath[minIndx] .Y));
                        else
                            this.SeedPoints[this.SeedPoints.Count - 1] = new PointF(this.TempPath[minIndx].X, this.TempPath[minIndx].Y);
                    }

                    List<PointF> arr = new List<PointF>();
                    for (int i = 0; i <= minIndx - 1; i++)
                        arr.Add(this.TempPath[i]);

                    if (this.cbAddLine.Checked && TempPath.Count == 2)
                    {
                        for (int i = minIndx; i <= TempPath.Count - 1; i++)
                            arr.Add(this.TempPath[i]);
                    }

                    this.CurPath?.Add(arr);

                    if (arr.Count > 0 && this.CurPath?.Count > 1)
                    {
                        List<PointF> l = this.CurPath[this.CurPath.Count - 2];
                        if (l[0].X == arr[0].X && l[0].Y == arr[0].Y)
                            this.CurPath.RemoveAt(this.CurPath.Count - 2);
                    }
                }
            }
        }

        public void AddLine(List<List<PointF>> curPath, List<PointF> seedPoints)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(seedPoints[seedPoints.Count - 2], seedPoints[seedPoints.Count - 1]);
                if (gp.PointCount > 0)
                {
                    gp.Flatten();
                    List<PointF> l = new List<PointF>();
                    l.AddRange(gp.PathPoints);
                    curPath.Add(l);
                }
            }
        }

        public List<PointF>? AddLine(List<PointF> tempPath, PointF pt, PointF pt2)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(pt2, pt);
                if (gp.PointCount > 0)
                {
                    gp.Flatten();
                    List<PointF> l = new List<PointF>();
                    l.AddRange(gp.PathPoints);
                    return l;
                }
            }

            return null;
        }

        public List<List<PointF>> ClonePath(List<List<PointF>>? curPath)
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();

            if (curPath != null && curPath.Count > 0)
            {
                for (int i = 0; i <= curPath.Count - 1; i++)
                {
                    List<PointF> p = curPath[i];

                    PointF[] l = new PointF[p.Count - 1 + 1];
                    p.CopyTo(l);
                    pathCopy.Add(l.ToList());
                }
            }

            return pathCopy;
        }

        public void CloneCurPath()
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();

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

        public List<List<PointF>>? ClonePath(List<List<PointF>> curPath, bool flatten, bool reducePoints, double minDist, double epsilon)
        {
            if (curPath != null)
            {
                if (curPath.Count > 0)
                {
                    List<List<PointF>> lOut = new List<List<PointF>>();
                    List<List<PointF>> lOut2 = new List<List<PointF>>();
                    for (int i = 0; i <= curPath.Count - 1; i++)
                    {
                        List<PointF> p = curPath[i];

                        if (p.Count > 0)
                        {
                            List<PointF> l = new List<PointF>();
                            l.AddRange(p);
                            lOut.Add(l);
                        }
                    }

                    if (flatten)
                    {
                        if (lOut != null && lOut.Count > 0)
                        {
                            List<PointF> l = new List<PointF>();
                            for (int i = 0; i <= lOut.Count - 1; i++)
                            {
                                List<PointF> p = lOut[i];

                                if (p != null && p.Count > 0)
                                {
                                    for (int j = 0; j <= p.Count - 1; j++)
                                        l.Add(new PointF(p[j].X, p[j].Y));
                                }
                            }
                            if (reducePoints && l.Count > 1)
                                l = ReducePathPoints(l, minDist, epsilon);
                            lOut2.Add(l);
                        }
                    }
                    else
                        lOut2 = lOut;

                    if (curPath[0].Count > 0 && curPath[curPath.Count - 1].Count > 0)
                    {
                        if (curPath[curPath.Count - 1][curPath[curPath.Count - 1].Count - 1].X == curPath[0][0].X && curPath[curPath.Count - 1][curPath[curPath.Count - 1].Count - 1].Y == curPath[0][0].Y)
                        {
                            if (lOut2.Count > 0 && lOut2[lOut2.Count - 1].Count > 0)
                                lOut2[lOut2.Count - 1].Add(new PointF(lOut2[0][0].X, lOut2[0][0].Y));
                        }
                    }

                    return lOut2;
                }
            }

            return null;
        }

        public List<PointF> ReducePathPoints(List<PointF> points, double minDist, double epsilon)
        {
            List<PointF> lOut = new List<PointF>();

            using (GraphicsPath gp = GetPath2(points, true, epsilon, false, true, false, minDist))
            {
                if (gp.PointCount > 1)
                {
                    PointF[] pts = gp.PathPoints;

                    lOut = new List<PointF>();
                    lOut.AddRange(pts.ToList());
                }
                else
                    MessageBox.Show("Path is empty");
            }

            return lOut;
        }

        public GraphicsPath GetPath2(List<PointF> curPath, bool smoothen, double epsilon, bool addcurves, bool remColinearity, bool getOutliers, double minDist)
        {
            GraphicsPath fPath = new GraphicsPath();
            List<PointF> lList2 = new List<PointF>();
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = false;

            List<PointF> fList = new List<PointF>();
            fList.AddRange(curPath);

            fList = CleanList(fList, minDist);

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

            return fPath;
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

        public bool CheckPaths(List<List<PointF>> curPath)
        {
            if (this.btnSavedPaths != null)
            {
                for (int i = 0; i < this.PathList?.Count; i++)
                {
                    if (curPath.Count != this.PathList?[i].Count)
                        continue;

                    List<List<PointF>> l1 = this.PathList[i];
                    bool lEq = false;
                    for (int j = 0; j <= l1.Count - 1; j++)
                    {
                        List<PointF> p = l1[j];
                        List<PointF> q = curPath[j];

                        if (p.Count != q.Count)
                        {
                            lEq = true;
                            break;
                        }

                        for (int jj = 0; jj <= p.Count - 1; jj++)
                        {
                            if (p[jj].X != q[jj].X || p[jj].Y != q[jj].Y)
                            {
                                lEq = true;
                                break;
                            }
                        }
                    }

                    if (lEq)
                        continue;

                    return true;
                }
            }

            return false;
        }
    }
}
