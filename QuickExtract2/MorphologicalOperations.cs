using ChainCodeFinder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace QuickExtract2
{
    internal class MorphologicalOperations
    {
        public GraphicsPathData ShiftCoords4(GraphicsPath? gP, float shiftInwards, bool mapLargestOnly, bool stretch, ShiftFractionMode fractionMode)
        {
            if (gP != null && gP.PointCount > 1)
            {
                List<List<byte>> pTAll = new List<List<byte>>();
                List<List<PointF>> pPAll = new List<List<PointF>>();
                AnalyzeAndSplit(gP, pPAll, pTAll);

                GraphicsPathData shiftedPath = new GraphicsPathData();

                for (int cnt2 = 0; cnt2 <= pPAll.Count - 1; cnt2++) // loop over all figures
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

                                        int breite = System.Convert.ToInt32(Math.Ceiling(shiftInwards)); // width to remove
                                        double fraction = shiftInwards / (double)breite != 0 ? breite : shiftInwards - (System.Convert.ToInt32(shiftInwards)); // the weight of the inner path when interpolating points

                                        if (shiftInwards < 0)
                                        {
                                            ShiftOutwards(shiftInwards, outerPath, outerPts, breite, rc, w, h);

                                            PointF[] outerPts2 = new PointF[outerPath.PathPoints.Length - 1 + 1];
                                            outerPath.PathPoints.CopyTo(outerPts2, 0);
                                            outerPts = outerPts2; // new points
                                            rc = outerPath.GetBounds(); // new bounds
                                            w = System.Convert.ToInt32(Math.Ceiling(rc.Width));
                                            h = System.Convert.ToInt32(Math.Ceiling(rc.Height));
                                            shiftInwards *= -1; // switch sign -> outerpath is now the extended outer path, original outerpath is inner path now
                                                                // fraction = 1.0 - fraction 'switch fraction

                                            // we extended by breite so we only need to shift back by the fraction
                                            shiftInwards -= System.Convert.ToSingle(Math.Floor(shiftInwards));
                                            breite = 1;
                                            fraction = 1.0 - shiftInwards;
                                        }

                                        if (outerPath.PointCount > 4)
                                        {
                                            List<PointF> shiftedPathPts = new List<PointF>();
                                            breite = Math.Abs(breite);

                                            if (AvailMem.AvailMem.checkAvailRam((w + 2 * breite) * (h + 2 * breite) * 8L))
                                            {
                                                try
                                                {
                                                    bmpTmp = new Bitmap(w + 2 * breite, h + 2 * breite);
                                                    using (Graphics g = Graphics.FromImage(bmpTmp))
                                                    {
                                                        // translate path close to origin, to be drawn entirely into the bmp
                                                        using (Matrix mx = new Matrix(1, 0, 0, 1, -rc.X + breite, -rc.Y + breite))
                                                        {
                                                            outerPath.Transform(mx);
                                                        }
                                                        g.Clear(Color.Black); // Get a black (bg) and white (fg) image for the erode method
                                                        g.FillPath(Brushes.White, outerPath);
                                                        // re-translate
                                                        using (Matrix mx = new Matrix(1, 0, 0, 1, rc.X - breite, rc.Y - breite))
                                                        {
                                                            outerPath.Transform(mx);
                                                        }

                                                        bmpInner = Erode(bmpTmp, breite, null); // remove the entire width

                                                        if (bmpInner != null)
                                                        {
                                                            // get the new outline (path) [optional: and prolongate it to have the same amount of points as the large path]
                                                            // then find the closest points and resample
                                                            // innerPath can have more figures than outerPath
                                                            List<ChainCode> lInner = GetBoundary(bmpInner); // current contours

                                                            // rem last point for each figure
                                                            for (int i = 0; i <= lInner.Count - 1; i++)
                                                            {
                                                                if (lInner[i].Coord[lInner[i].Coord.Count - 1].X == lInner[i].Coord[0].X && lInner[i].Coord[lInner[i].Coord.Count - 1].Y == lInner[i].Coord[0].Y)
                                                                    lInner[i].Coord.RemoveAt(lInner[i].Coord.Count - 1);
                                                            }

                                                            if (lInner.Count > 0)
                                                            {
                                                                lInner = lInner.OrderByDescending(a => a.Coord.Count).ToList();

                                                                // get all points
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

                                                                // relation outer point amount to inner point amount (if we stretch the inner array to the length of the outer one)
                                                                double factor = stretch ? outerPath.PointCount / (double)lInner.Sum(a => a.Coord.Count) : 1.0;
                                                                double dFracSum = 0.0; // value that sums the accumulated fractions (differences of exact product to index in array)
                                                                int pointCountAll = 0; // accumulated pointsamount

                                                                for (int l = 0; l <= newInnerPtsAll.Count - 1; l++)
                                                                {
                                                                    double dLength = newInnerPtsAll[l].Count * factor; // "exact" location of start of current figure in [maybe] elongated array
                                                                    int pointCount = System.Convert.ToInt32(Math.Ceiling(dLength)); // integer (real) location
                                                                    dFracSum += pointCount - dLength; // add up the difference

                                                                    if (dFracSum > 1.0)
                                                                    {
                                                                        dFracSum -= 1;
                                                                        pointCount -= 1;
                                                                    }

                                                                    pointCountAll += pointCount; // add up pointsamount

                                                                    if (pointCountAll > outerPath.PointCount)
                                                                        pointCount -= pointCountAll - outerPath.PointCount;

                                                                    newInnerPtsAll[l] = TranslateNewInnerPath(newInnerPtsAll[l], rc, breite); // translate back (we got the new innerpath from the bmp, which holds the translated path)
                                                                    List<PointF> newInnerPathPoints = newInnerPtsAll[l];
                                                                    // lets have a dx or dy at all points when comparing neighbors
                                                                    newInnerPathPoints = newInnerPathPoints.Distinct().ToList();

                                                                    if (stretch)
                                                                        ProlongateInnerPath(newInnerPtsAll[l], pointCount, factor);// if wanted, elongate the innerpath figure

                                                                    double outerFraction = 1.0 - fraction; // weight of outerpath in interpolating points

                                                                    pTAll[cnt2][shiftedPathPts.Count - System.Convert.ToInt32(Math.Floor(dFracSum))] = 0; // first byte for new figure in pathtypes list

                                                                    GetNewAndInterpolatedPoints(outerPath, newInnerPathPoints, shiftedPathPts, outerPts, fractionMode, breite, w, h, fraction, outerFraction, rc); // do the work
                                                                }

                                                                // do a check for equal lengths
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

                                            // add current results to the resulting large pathdata
                                            if (shiftedPathPts.Count > 1)
                                            {
                                                if (stretch)
                                                {
                                                    // make sure length of both arrays are the same
                                                    if (stretch && shiftedPathPts.Count != pTAll[cnt2].Count)
                                                        shiftedPathPts = RestrictOuterPath(shiftedPathPts, pTAll[cnt2].Count);

                                                    int cnt4 = 0;

                                                    while (cnt4 < pTAll[cnt2].Count)
                                                    {
                                                        List<byte> pT = new List<byte>();
                                                        List<PointF> pP = new List<PointF>();

                                                        pP.Add(shiftedPathPts[cnt4]);
                                                        pT.Add(0);
                                                        cnt4 += 1;

                                                        while (cnt4 < pTAll[cnt2].Count && pTAll[cnt2][cnt4] != 0)
                                                        {
                                                            pP.Add(shiftedPathPts[cnt4]);
                                                            pT.Add(pTAll[cnt2][cnt4]);
                                                            cnt4 += 1;
                                                        }

                                                        pT[pT.Count - 1] = 129;
                                                        shiftedPath.Add(pP, pT);
                                                    }
                                                }
                                                else
                                                {
                                                    // setup and fill the data lists
                                                    int cnt4 = 0;
                                                    List<byte> pT = new List<byte>();
                                                    List<PointF> pP = new List<PointF>();
                                                    while (cnt4 < shiftedPathPts.Count)
                                                    {
                                                        pP.Add(shiftedPathPts[cnt4]);
                                                        pT.Add(0);
                                                        cnt4 += 1;
                                                    }

                                                    pT[pT.Count - 1] = 129; // ensure close
                                                    shiftedPath.Add(pP, pT);
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
                return new GraphicsPathData();
        }

        private void GetNewAndInterpolatedPoints(GraphicsPath outerPath, List<PointF> newInnerPathPoints, List<PointF> shiftedPathPts, PointF[] outerPts, ShiftFractionMode fractionMode, int breite, int w, int h, double fraction, double outerFraction, RectangleF rc)
        {
            if (fractionMode == ShiftFractionMode.FollowNormal)
            {
                int w2 = w;
                int h2 = h;

                // draw the outerpath to a bitmap
                if (AvailMem.AvailMem.checkAvailRam(w2 * h2 * 8L))
                {
                    using (Bitmap bmpTmp2 = new Bitmap(w2, h2))
                    {
                        using (Matrix mx = new Matrix(1, 0, 0, 1, -rc.X, -rc.Y))
                        {
                            outerPath.Transform(mx);
                        }

                        using (Graphics gx = Graphics.FromImage(bmpTmp2))
                        {
                            gx.Clear(Color.Transparent);
                            gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gx.SmoothingMode = SmoothingMode.AntiAlias;

                            using (Pen pen = new Pen(Color.Black, 1))
                            {
                                pen.LineJoin = LineJoin.Round;
                                gx.DrawPath(pen, outerPath);
                            }
                        }

                        using (Matrix mx = new Matrix(1, 0, 0, 1, rc.X, rc.Y))
                        {
                            outerPath.Transform(mx);
                        }

                        BitmapData? bmData = null;
                        byte[] p = new byte[w2 * h2 * 4 - 1 + 1];
                        int stride = 0;

                        try
                        {
                            bmData = bmpTmp2.LockBits(new Rectangle(0, 0, w2, h2), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            stride = bmData.Stride;
                            Marshal.Copy(bmData.Scan0, p, 0, p.Length);
                            bmpTmp2.UnlockBits(bmData);
                        }
                        catch
                        {
                            try
                            {
                                if (bmData != null)
                                    bmpTmp2.UnlockBits(bmData);
                            }
                            catch
                            {
                            }
                        }

                        for (int i = 0; i <= newInnerPathPoints.Count - 1; i++)
                        {
                            double dxN = 0;
                            double dyN = 0;

                            // get the slope's dx & dy
                            if (newInnerPathPoints.Count > 2 && newInnerPathPoints[newInnerPathPoints.Count - 1].X == newInnerPathPoints[0].X && newInnerPathPoints[newInnerPathPoints.Count - 1].Y == newInnerPathPoints[0].Y)
                            {
                                if (i == 0)
                                {
                                    dxN = (newInnerPathPoints[i].X - newInnerPathPoints[newInnerPathPoints.Count - 2].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                                    dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[newInnerPathPoints.Count - 2].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                                }
                                else if (i > 0 && i < newInnerPathPoints.Count - 1)
                                {
                                    dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                                    dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                                }
                                else if (i == newInnerPathPoints.Count - 1)
                                {
                                    dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[1].X);
                                    dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[1].Y);
                                }
                            }
                            else if (i == 0)
                            {
                                dxN = (newInnerPathPoints[i].X - newInnerPathPoints[newInnerPathPoints.Count - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                                dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[newInnerPathPoints.Count - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                            }
                            else if (i > 0 && i < newInnerPathPoints.Count - 1)
                            {
                                dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                                dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                            }
                            else if (i == newInnerPathPoints.Count - 1)
                            {
                                dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[0].X);
                                dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[0].Y);
                            }

                            // normalize
                            double r = Math.Max(Math.Abs(dxN), Math.Abs(dyN));
                            dxN /= r;
                            dyN /= r;

                            float innerX = newInnerPathPoints[i].X - rc.X + 0.5F; // fixed add (needed by drawing path antialiased)
                            float innerY = newInnerPathPoints[i].Y - rc.Y + 0.5F;

                            float outerX = innerX;
                            float outerY = innerY;

                            int outerXF = Math.Min(Math.Max(System.Convert.ToInt32(Math.Floor(outerX)), 0), w2 - 1);
                            int outerXC = Math.Min(System.Convert.ToInt32(Math.Ceiling(outerX)), w2 - 1);
                            int outerYF = Math.Min(Math.Max(System.Convert.ToInt32(Math.Floor(outerY)), 0), h2 - 1);
                            int outerYC = Math.Min(System.Convert.ToInt32(Math.Ceiling(outerY)), h2 - 1);
                            int cntPic = 0;

                            // now search the bitmap for the outerCoord starting at the innerCoord, stepping in normal direction
                            try
                            {
                                if (dxN > dyN)
                                {
                                    while (p[outerYF * stride + outerXF * 4 + 3] == 0 && p[outerYF * stride + outerXC * 4 + 3] == 0 && cntPic < breite)
                                    {
                                        outerX = System.Convert.ToSingle(outerX + dyN);
                                        outerY = System.Convert.ToSingle(outerY + dxN);

                                        outerXF = Math.Min(Math.Max(System.Convert.ToInt32(Math.Floor(outerX)), 0), w2 - 1);
                                        outerXC = Math.Min(System.Convert.ToInt32(Math.Ceiling(outerX)), w2 - 1);
                                        outerYF = Math.Min(Math.Max(System.Convert.ToInt32(Math.Floor(outerY)), 0), h2 - 1);
                                        outerYC = Math.Min(System.Convert.ToInt32(Math.Ceiling(outerY)), h2 - 1);

                                        cntPic += 1;
                                    }
                                }
                                else
                                    while (p[outerYF * stride + outerXF * 4 + 3] == 0 && p[outerYC * stride + outerXF * 4 + 3] == 0 && cntPic < breite)
                                    {
                                        outerX = System.Convert.ToSingle(outerX + dyN);
                                        outerY = System.Convert.ToSingle(outerY + dxN);

                                        outerXF = Math.Min(Math.Max(System.Convert.ToInt32(Math.Floor(outerX)), 0), w2 - 1);
                                        outerXC = Math.Min(System.Convert.ToInt32(Math.Ceiling(outerX)), w2 - 1);
                                        outerYF = Math.Min(Math.Max(System.Convert.ToInt32(Math.Floor(outerY)), 0), h2 - 1);
                                        outerYC = Math.Min(System.Convert.ToInt32(Math.Ceiling(outerY)), h2 - 1);

                                        cntPic += 1;
                                    }
                            }
                            catch
                            {
                            }

                            // interpolate linearly
                            double newX = innerX * fraction + outerX * outerFraction;
                            double newY = innerY * fraction + outerY * outerFraction;

                            // add to list
                            shiftedPathPts.Add(new PointF(System.Convert.ToSingle(newX + rc.X), System.Convert.ToSingle(newY + rc.Y)));
                        }
                    }
                }
            }
            else
                // since the shift back is less or equal 1, loop over the innerPoints and set FillMode to Winding
                // for keeping the amount of figures of the innerPath
                for (int i = 0; i <= newInnerPathPoints.Count - 1; i++)
                {
                    double minDist = double.MaxValue; // find closest point innouterpath
                    int minIndex = -1;

                    for (int j = 0; j <= outerPts.Length - 1; j++)
                    {
                        double dx = outerPts[j].X - newInnerPathPoints[i].X;
                        double dy = outerPts[j].Y - newInnerPathPoints[i].Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy); // get distance of inner to outer point(s)

                        if (dist < minDist)
                        {
                            minDist = dist;
                            minIndex = j;
                        }
                    }

                    // Note: this is just a rough estimation of the correct index, when the (integer-) shift is done in both directions.
                    // We just multiply the smaller slope-direction of the inner path with the large shift direction and 
                    // subtract the current shift in that small direction to get an estimate of how far the correct index is away 
                    // from our current one which represents the shortest distance between outer and inner path.
                    // This is more or less fun, I will do some tests, and if the results are good, I'll keep it, else we would have to
                    // follow the normal of the inner path at the current coord until it crosses the outerpath.
                    int addX = 0;
                    int addY = 0;

                    if (fractionMode == ShiftFractionMode.ClosestAndEstimate)
                    {
                        double dxN = 0;
                        double dyN = 0;

                        if (newInnerPathPoints.Count > 2 && newInnerPathPoints[newInnerPathPoints.Count - 1].X == newInnerPathPoints[0].X && newInnerPathPoints[newInnerPathPoints.Count - 1].Y == newInnerPathPoints[0].Y)
                        {
                            if (i == 0)
                            {
                                dxN = (newInnerPathPoints[i].X - newInnerPathPoints[newInnerPathPoints.Count - 2].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                                dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[newInnerPathPoints.Count - 2].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                            }
                            else if (i > 0 && i < newInnerPathPoints.Count - 1)
                            {
                                dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                                dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                            }
                            else if (i == newInnerPathPoints.Count - 1)
                            {
                                dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[1].X);
                                dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[1].Y);
                            }
                        }
                        else if (i == 0)
                        {
                            dxN = (newInnerPathPoints[i].X - newInnerPathPoints[newInnerPathPoints.Count - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                            dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[newInnerPathPoints.Count - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                        }
                        else if (i > 0 && i < newInnerPathPoints.Count - 1)
                        {
                            dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[i + 1].X);
                            dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[i + 1].Y);
                        }
                        else if (i == newInnerPathPoints.Count - 1)
                        {
                            dxN = (newInnerPathPoints[i].X - newInnerPathPoints[i - 1].X) - (newInnerPathPoints[i].X - newInnerPathPoints[0].X);
                            dyN = (newInnerPathPoints[i].Y - newInnerPathPoints[i - 1].Y) - (newInnerPathPoints[i].Y - newInnerPathPoints[0].Y);
                        }
                        double r = Math.Max(Math.Abs(dxN), Math.Abs(dyN));
                        dxN /= r;
                        dyN /= r;

                        double dx = (outerPts[minIndex].X - newInnerPathPoints[i].X);
                        double dy = (outerPts[minIndex].Y - newInnerPathPoints[i].Y);

                        double r2 = Math.Max(Math.Abs(dx), Math.Abs(dy));

                        // we assume that in the outerpoints each point differs 1 from the point before,
                        // so we estimate the correct index by:
                        if (dx / r2 != -dyN || dy / r2 != dxN)
                        {
                            if (dxN > dyN)
                                addX = System.Convert.ToInt32(dy * -dyN - dx); // large direction * small offset - current distance
                            else
                                addY = System.Convert.ToInt32(dx * -dxN - dy);
                        }
                    }

                    if (minIndex > -1)
                    {
                        float outerX = outerPts[minIndex].X;
                        float outerY = outerPts[minIndex].Y;

                        if (addX != 0)
                        {
                            if (minIndex + addX >= 0 && minIndex + addX < outerPts.Length)
                                outerX = outerPts[minIndex + addX].X;
                            else
                            {
                                if (minIndex + addX < 0)
                                    outerX = outerPts[outerPts.Length + (minIndex + addX)].X;
                                if (minIndex + addX > outerPts.Length - 1)
                                    outerX = outerPts[minIndex + addX - outerPts.Length].X;
                            }
                        }
                        if (addY != 0)
                        {
                            if (minIndex + addY >= 0 && minIndex + addY < outerPts.Length)
                                outerY = outerPts[minIndex + addY].Y;
                            else
                            {
                                if (minIndex + addY < 0)
                                    outerY = outerPts[outerPts.Length + (minIndex + addY)].Y;
                                if (minIndex + addY > outerPts.Length - 1)
                                    outerY = outerPts[minIndex + addY - outerPts.Length].Y;
                            }
                        }

                        float innerX = newInnerPathPoints[i].X;
                        float innerY = newInnerPathPoints[i].Y;

                        // interpolate linearly
                        double newX = innerX * fraction + outerX * outerFraction;
                        double newY = innerY * fraction + outerY * outerFraction;

                        // add to list
                        shiftedPathPts.Add(new PointF(System.Convert.ToSingle(newX), System.Convert.ToSingle(newY)));
                    }
                }
        }

        private void AnalyzeAndSplit(GraphicsPath gP, List<List<PointF>> pPAll, List<List<byte>> pTAll)
        {
            // do it separately for each PathFigure...
            byte[] t = new byte[gP.PathTypes.Length - 1 + 1];
            gP.PathTypes.CopyTo(t, 0);
            PointF[] pts = new PointF[gP.PathPoints.Length - 1 + 1];
            gP.PathPoints.CopyTo(pts, 0);

            int cnt = 0;

            while (cnt < t.Length)
            {
                List<byte> pT = new List<byte>();
                List<PointF> pP = new List<PointF>();

                pP.Add(pts[cnt]);
                pT.Add(0);  // start figure
                cnt += 1;

                while (cnt < t.Length && t[cnt] != 0) // if 0, new figure
                {
                    pP.Add(pts[cnt]);
                    pT.Add(t[cnt]);
                    cnt += 1;
                }

                pPAll.Add(pP);
                pT[pT.Count - 1] = 129; // close figure
                pTAll.Add(pT);
            }

            pPAll = pPAll.OrderByDescending(a => a.Count).ToList();
            pTAll = pTAll.OrderByDescending(a => a.Count).ToList();
        }

        private void ShiftOutwards(float shiftInwards, GraphicsPath outerPath, PointF[] outerPts, int breite, RectangleF rc, int w, int h)
        {
            Bitmap? bmpTmp2 = null;
            Bitmap? bmpOuter = null;

            try
            {
                int br = System.Convert.ToInt32(Math.Floor(shiftInwards)); // width
                if (AvailMem.AvailMem.checkAvailRam((w - br * 2) * (h - br * 2) * 4L))
                {
                    bmpTmp2 = new Bitmap(w - br * 2, h - br * 2);
                    using (GraphicsPath outerPath2 = (GraphicsPath)outerPath.Clone())
                    {
                        using (Pen p = new Pen(Color.Black, -br))
                        {
                            using (Matrix mx = new Matrix(1, 0, 0, 1, -rc.X - br, -rc.Y - br)) // translate close to origin -> so we can draw the entire path to the bmp
                            {
                                outerPath2.Transform(mx);
                            }

                            using (Graphics g = Graphics.FromImage(bmpTmp2)) // Get a black (bg) and white (fg) image for the dilate method
                            {
                                g.Clear(Color.Black);
                                g.FillPath(Brushes.White, outerPath2);
                            }

                            bmpOuter = Dilate(bmpTmp2, -breite + 1, null); // add outline

                            if (bmpOuter != null)
                            {
                                List<ChainCodeF> lOuter = GetBoundaryF(bmpOuter); // get the new outline

                                outerPath.Reset(); // refill the new outer path
                                for (int i = 0; i <= lOuter.Count - 1; i++)
                                {
                                    // translate
                                    for (int j = 0; j <= lOuter[i].Coord.Count - 1; j++)
                                        lOuter[i].Coord[j] = new PointF(lOuter[i].Coord[j].X + rc.X + br, lOuter[i].Coord[j].Y + rc.Y + br);

                                    if (lOuter[i].Coord[lOuter[i].Coord.Count - 1].X == lOuter[i].Coord[0].X && lOuter[i].Coord[lOuter[i].Coord.Count - 1].Y == lOuter[i].Coord[0].Y)
                                        lOuter[i].Coord.RemoveAt(lOuter[i].Coord.Count - 1);

                                    outerPath.AddLines(lOuter[i].Coord.ToArray());
                                    outerPath.CloseFigure();
                                }
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

        private Bitmap? Erode(Bitmap bmpTmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            Bitmap? bmpWork = null;
            Bitmap? bmpWork2 = null;
            Bitmap? bmpOut = null;

            breite = Math.Abs(breite);

            try
            {
                bool invert = false;
                bmpWork = bmpTmp;

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(20);

                bool binary = false;
                int fgVal = 127;

                int radiusX = breite;
                int radiusY = breite;

                Tuple<bool, bool, bool> channels = new Tuple<bool, bool, bool>(true, true, true);

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(40);

                if (invert)
                    this.Invert(bmpWork);

                bmpWork2 = ExtractChannels(bmpWork, channels.Item1, channels.Item2, channels.Item3);

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(60);

                bmpOut = Erode(bmpWork2, radiusX, radiusY, binary, fgVal, null, null);

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(100);

                if (invert && bmpOut != null)
                    this.Invert(bmpOut);

                return bmpOut;
            }
            catch
            {
                if (bmpOut != null)
                {
                    bmpOut.Dispose();
                    bmpOut = null;
                }
            }
            finally
            {
                if (bmpWork != null)
                {
                    bmpWork.Dispose();
                    bmpWork = null;
                }
                if (bmpWork2 != null)
                {
                    bmpWork2.Dispose();
                    bmpWork2 = null;
                }
            }

            return null;
        }

        private Bitmap? Dilate(Bitmap bmpTmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            Bitmap? bmpWork = null;
            Bitmap? bmpWork2 = null;
            Bitmap? bmpOut = null;

            try
            {
                bool invert = false;
                bmpWork = bmpTmp;

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(20);

                bool binary = false;
                int fgVal = 127;

                int radiusX = breite;
                int radiusY = breite;

                Tuple<bool, bool, bool> channels = new Tuple<bool, bool, bool>(true, true, true);

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(40);

                if (invert)
                    this.Invert(bmpWork);

                bmpWork2 = ExtractChannels(bmpWork, channels.Item1, channels.Item2, channels.Item3);

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(60);

                bmpOut = this.Dilate(bmpWork2, radiusX, radiusY, binary, fgVal, null, null);

                if (bgw != null && bgw.WorkerReportsProgress)
                    bgw.ReportProgress(100);

                if (invert && bmpOut != null)
                    this.Invert(bmpOut);

                return bmpOut;
            }
            catch
            {
                if (bmpOut != null)
                {
                    bmpOut.Dispose();
                    bmpOut = null;
                }
            }
            finally
            {
                if (bmpWork != null)
                {
                    bmpWork.Dispose();
                    bmpWork = null;
                }
                if (bmpWork2 != null)
                {
                    bmpWork2.Dispose();
                    bmpWork2 = null;
                }
            }

            return null;
        }

        public static Bitmap? ExtractChannels(Bitmap bmp, bool r, bool g, bool b)
        {
            BitmapData? bmData = null;
            BitmapData? bmData2 = null;

            if (!AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                return null;

            Bitmap? bOut = null;

            try
            {
                bOut = new Bitmap(bmp.Width, bmp.Height);
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                bmData2 = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int w = bmp.Width;
                int h = bmp.Height;
                int stride = bmData.Stride;

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);
                byte[] p2 = new byte[(bmData2.Stride * bmData2.Height) - 1 + 1];
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length);

                if (r && !g && !b)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            p2[pos + 0] = System.Convert.ToByte(p[pos + 2]);
                            p2[pos + 1] = System.Convert.ToByte(p[pos + 2]);
                            p2[pos + 2] = System.Convert.ToByte(p[pos + 2]);
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }
                else if (g && !r && !b)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            p2[pos + 0] = System.Convert.ToByte(p[pos + 1]);
                            p2[pos + 1] = System.Convert.ToByte(p[pos + 1]);
                            p2[pos + 2] = System.Convert.ToByte(p[pos + 1]);
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }
                else if (b && !g && !r)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            p2[pos + 0] = System.Convert.ToByte(p[pos]);
                            p2[pos + 1] = System.Convert.ToByte(p[pos]);
                            p2[pos + 2] = System.Convert.ToByte(p[pos]);
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }
                else if (r && g && !b)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            int value = System.Convert.ToInt32(System.Convert.ToDouble(p[pos + 1]) * 0.663 + System.Convert.ToDouble(p[pos + 2]) * 0.337);

                            if (value < 0)
                                value = 0;
                            if (value > 255)
                                value = 255;

                            byte v = System.Convert.ToByte(value);
                            p2[pos] = v;
                            p2[pos + 1] = v;
                            p2[pos + 2] = v;
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }
                else if (r && b && !g)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            int value = System.Convert.ToInt32(System.Convert.ToDouble(p[pos]) * 0.276 + System.Convert.ToDouble(p[pos + 2]) * 0.724);

                            if (value < 0)
                                value = 0;
                            if (value > 255)
                                value = 255;

                            byte v = System.Convert.ToByte(value);
                            p2[pos] = v;
                            p2[pos + 1] = v;
                            p2[pos + 2] = v;
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }
                else if (g && b && !r)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            int value = System.Convert.ToInt32(System.Convert.ToDouble(p[pos]) * 0.163 + System.Convert.ToDouble(p[pos + 1]) * 0.837);

                            if (value < 0)
                                value = 0;
                            if (value > 255)
                                value = 255;

                            byte v = System.Convert.ToByte(value);
                            p2[pos] = v;
                            p2[pos + 1] = v;
                            p2[pos + 2] = v;
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }
                else if (r && g && b)
                {
                    // for (int y = 0; y < bmData.Height; y++)
                    Parallel.For(0, h, y =>
                    {
                        int pos = y * stride;

                        for (int x = 0; x <= w - 1; x++)
                        {
                            int value = System.Convert.ToInt32(System.Convert.ToDouble(p[pos]) * 0.114 + System.Convert.ToDouble(p[pos + 1]) * 0.587 + System.Convert.ToDouble(p[pos + 2]) * 0.299);

                            if (value < 0)
                                value = 0;
                            if (value > 255)
                                value = 255;

                            byte v = System.Convert.ToByte(value);
                            p2[pos] = v;
                            p2[pos + 1] = v;
                            p2[pos + 2] = v;
                            p2[pos + 3] = System.Convert.ToByte(p[pos + 3]);

                            pos += 4;
                        }
                    });
                }

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length);
                bmp.UnlockBits(bmData);
                bOut.UnlockBits(bmData2);
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
                try
                {
                    if (bmData2 != null && bOut != null)
                        bOut.UnlockBits(bmData2);
                }
                catch
                {
                }
            }

            return bOut;
        }

        public static Bitmap? Erode(Bitmap? bmp, int radiusX, int radiusY, bool returnBinary, int th, System.ComponentModel.BackgroundWorker? bgw, ProgressEventArgs? pe)
        {
            if (bmp != null && AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L))
            {
                BitmapData? bmData = null;
                BitmapData? bmData2 = null;
                BitmapData? bmDataTmp = null;
                Bitmap? bOut = null;
                Bitmap? bTmp = null;
                try
                {
                    bOut = new Bitmap(bmp.Width, bmp.Height);
                    bTmp = new Bitmap(bmp.Width, bmp.Height);
                    bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    bmData2 = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    bmDataTmp = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);
                    byte[] p2 = new byte[(bmData2.Stride * bmData2.Height) - 1 + 1];
                    Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length);
                    byte[] pTmp = new byte[(bmDataTmp.Stride * bmDataTmp.Height) - 1 + 1];
                    Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length);

                    int nWidth = bmp.Width;
                    int nHeight = bmp.Height;
                    int stride = bmData.Stride;

                    if (returnBinary)
                    {
                        // For y As Integer = 0 To nHeight - 1
                        Parallel.For(0, nHeight, y =>
                        {
                            for (int x = 0; x <= nWidth - 1; x++)
                            {
                                byte b = 255;
                                for (int i = x - radiusX; i <= x + radiusX; i++)
                                {
                                    if (i >= 0 && i < nWidth)
                                    {
                                        byte b2 = 0;
                                        if (p[y * stride + i * 4] > th)
                                            b2 = 255;

                                        b = Math.Min(b, b2);
                                    }
                                }
                                pTmp[y * stride + x * 4] = b;
                                pTmp[y * stride + x * 4 + 1] = b;
                                pTmp[y * stride + x * 4 + 2] = b;
                                pTmp[y * stride + x * 4 + 3] = p[y * stride + x * 4 + 3];
                            }
                        });
                        // Next

                        Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length);

                        // For x As Integer = 0 To nWidth - 1
                        Parallel.For(0, nWidth, x =>
                        {
                            for (int y = 0; y <= nHeight - 1; y++)
                            {
                                byte b = 255;
                                for (int i = y - radiusY; i <= y + radiusY; i++)
                                {
                                    if (i >= 0 && i < nHeight)
                                        // Dim b2 As Byte = 0
                                        // If pTmp(i * stride + x * 4) > th Then
                                        // b2 = 255
                                        // End If
                                        // If i >= 0 AndAlso i < nHeight Then
                                        // b = Math.Min(b, b2)
                                        // End If   

                                        b = Math.Min(b, pTmp[i * stride + x * 4]);
                                }
                                p2[y * stride + x * 4] = b;
                                p2[y * stride + x * 4 + 1] = b;
                                p2[y * stride + x * 4 + 2] = b;
                                p2[y * stride + x * 4 + 3] = pTmp[y * stride + x * 4 + 3];
                            }
                        });
                    }
                    else
                    {
                        // For y As Integer = 0 To nHeight - 1
                        Parallel.For(0, nHeight, y =>
                        {
                            for (int x = 0; x <= nWidth - 1; x++)
                            {
                                byte b = 255;
                                for (int i = x - radiusX; i <= x + radiusX; i++)
                                {
                                    if (i >= 0 && i < nWidth)
                                        b = Math.Min(b, p[y * stride + i * 4]);
                                }
                                pTmp[y * stride + x * 4] = b;
                                pTmp[y * stride + x * 4 + 1] = b;
                                pTmp[y * stride + x * 4 + 2] = b;
                                pTmp[y * stride + x * 4 + 3] = p[y * stride + x * 4 + 3];
                            }
                        });
                        // Next

                        Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length);

                        // For x As Integer = 0 To nWidth - 1
                        Parallel.For(0, nWidth, x =>
                        {
                            for (int y = 0; y <= nHeight - 1; y++)
                            {
                                byte b = 255;
                                for (int i = y - radiusY; i <= y + radiusY; i++)
                                {
                                    if (i >= 0 && i < nHeight)
                                        b = Math.Min(b, pTmp[i * stride + x * 4]);
                                }
                                p2[y * stride + x * 4] = b;
                                p2[y * stride + x * 4 + 1] = b;
                                p2[y * stride + x * 4 + 2] = b;
                                p2[y * stride + x * 4 + 3] = pTmp[y * stride + x * 4 + 3];
                            }
                        });
                    }

                    Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length);

                    bmp.UnlockBits(bmData);
                    bOut.UnlockBits(bmData2);
                    bTmp.UnlockBits(bmDataTmp);
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
                    try
                    {
                        if (bmData2 != null && bOut != null)
                            bOut.UnlockBits(bmData2);
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (bmDataTmp != null && bTmp != null)
                            bTmp.UnlockBits(bmDataTmp);
                    }
                    catch
                    {
                    }
                    if (bOut != null)
                    {
                        bOut.Dispose();
                        bOut = null;
                    }
                }
                finally
                {
                    if (bTmp != null)
                    {
                        bTmp.Dispose();
                        bTmp = null;
                    }
                }

                return bOut;
            }
            return null;
        }

        public Bitmap? Dilate(Bitmap? bmp, int radiusX, int radiusy, bool returnBinary, int th, System.ComponentModel.BackgroundWorker? bgw, ProgressEventArgs? pe)
        {
            if (bmp != null && AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L))
            {
                BitmapData? bmData = null;
                BitmapData? bmData2 = null;
                BitmapData? bmDataTmp = null;
                Bitmap? bOut = null;
                Bitmap? bTmp = null;
                try
                {
                    bOut = new Bitmap(bmp.Width, bmp.Height);
                    bTmp = new Bitmap(bmp.Width, bmp.Height);
                    bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    bmData2 = bOut.LockBits(new Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    bmDataTmp = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);
                    byte[] p2 = new byte[(bmData2.Stride * bmData2.Height) - 1 + 1];
                    Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length);
                    byte[] pTmp = new byte[(bmDataTmp.Stride * bmDataTmp.Height) - 1 + 1];
                    Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length);

                    int nWidth = bmp.Width;
                    int nHeight = bmp.Height;
                    int stride = bmData.Stride;

                    if (returnBinary)
                    {
                        // For y As Integer = 0 To nHeight - 1
                        Parallel.For(0, nHeight, y =>
                        {
                            for (int x = 0; x <= nWidth - 1; x++)
                            {
                                byte b = 0;
                                for (int i = x - radiusX; i <= x + radiusX; i++)
                                {
                                    if (i >= 0 && i < nWidth)
                                    {
                                        byte b2 = 0;
                                        if (p[y * stride + i * 4] > th)
                                            b2 = 255;

                                        b = Math.Max(b, b2);
                                    }
                                }
                                pTmp[y * stride + x * 4] = b;
                                pTmp[y * stride + x * 4 + 1] = b;
                                pTmp[y * stride + x * 4 + 2] = b;
                                pTmp[y * stride + x * 4 + 3] = p[y * stride + x * 4 + 3];
                            }
                        });
                        // Next

                        Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length);

                        // For x As Integer = 0 To nWidth - 1
                        Parallel.For(0, nWidth, x =>
                        {
                            for (int y = 0; y <= nHeight - 1; y++)
                            {
                                byte b = 0;
                                for (int i = y - radiusy; i <= y + radiusy; i++)
                                {
                                    if (i >= 0 && i < nHeight)
                                        // Dim b2 As Byte = 0
                                        // If pTmp(i * stride + x * 4) > th Then
                                        // b2 = 255
                                        // End If
                                        // If i >= 0 AndAlso i < nHeight Then
                                        // b = Math.Max(b, b2)
                                        // End If

                                        b = Math.Max(b, pTmp[i * stride + x * 4]);
                                }
                                p2[y * stride + x * 4] = b;
                                p2[y * stride + x * 4 + 1] = b;
                                p2[y * stride + x * 4 + 2] = b;
                                p2[y * stride + x * 4 + 3] = pTmp[y * stride + x * 4 + 3];
                            }
                        });
                    }
                    else
                    {
                        // For y As Integer = 0 To nHeight - 1
                        Parallel.For(0, nHeight, y =>
                        {
                            for (int x = 0; x <= nWidth - 1; x++)
                            {
                                byte b = 0;
                                for (int i = x - radiusX; i <= x + radiusX; i++)
                                {
                                    if (i >= 0 && i < nWidth)
                                        b = Math.Max(b, p[y * stride + i * 4]);
                                }
                                pTmp[y * stride + x * 4] = b;
                                pTmp[y * stride + x * 4 + 1] = b;
                                pTmp[y * stride + x * 4 + 2] = b;
                                pTmp[y * stride + x * 4 + 3] = p[y * stride + x * 4 + 3];
                            }
                        });
                        // Next

                        Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length);

                        // For x As Integer = 0 To nWidth - 1
                        Parallel.For(0, nWidth, x =>
                        {
                            for (int y = 0; y <= nHeight - 1; y++)
                            {
                                byte b = 0;
                                for (int i = y - radiusy; i <= y + radiusy; i++)
                                {
                                    if (i >= 0 && i < nHeight)
                                        b = Math.Max(b, pTmp[i * stride + x * 4]);
                                }
                                p2[y * stride + x * 4] = b;
                                p2[y * stride + x * 4 + 1] = b;
                                p2[y * stride + x * 4 + 2] = b;
                                p2[y * stride + x * 4 + 3] = pTmp[y * stride + x * 4 + 3];
                            }
                        });
                    }

                    Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length);

                    bmp.UnlockBits(bmData);
                    bOut.UnlockBits(bmData2);
                    bTmp.UnlockBits(bmDataTmp);
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
                    try
                    {
                        if (bmData2 != null && bOut != null)
                            bOut.UnlockBits(bmData2);
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (bmDataTmp != null && bTmp != null)
                            bTmp.UnlockBits(bmDataTmp);
                    }
                    catch
                    {
                    }
                    if (bOut != null)
                    {
                        bOut.Dispose();
                        bOut = null;
                    }
                }
                finally
                {
                    if (bTmp != null)
                    {
                        bTmp.Dispose();
                        bTmp = null;
                    }
                }

                return bOut;
            }
            return null;
        }

        public void Invert(Bitmap bmp)
        {
            BitmapData? bmData = null;

            if (!AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                return;

            try
            {
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                int w = bmp.Width;
                int h = bmp.Height;
                int stride = bmData.Stride;

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                // for (int y = 0; y < bmData.Height; y++)
                Parallel.For(0, h, y =>
                {
                    int pos = y * stride;

                    for (int x = 0; x <= w - 1; x++)
                    {
                        p[pos + 0] = System.Convert.ToByte(255 - p[pos + 0]);
                        p[pos + 1] = System.Convert.ToByte(255 - p[pos + 1]);
                        p[pos + 2] = System.Convert.ToByte(255 - p[pos + 2]);

                        pos += 4;
                    }
                });

                Marshal.Copy(p, 0, bmData.Scan0, p.Length);
                bmp.UnlockBits(bmData);
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

        private List<ChainCodeF> GetBoundaryF(Bitmap upperImg)
        {
            ChainFinder cf = new ChainFinder();
            bool anc = cf.AllowNullCells;
            cf.AllowNullCells = true;
            List<ChainCodeF> l = cf.GetOutline(0, upperImg, 127, true, false);
            cf.AllowNullCells = anc;
            return l;
        }

        internal List<ChainCode> GetBoundary(Bitmap upperImg)
        {
            ChainFinder cf = new ChainFinder();
            bool anc = cf.AllowNullCells;
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(upperImg, 127, true, 0, false, 0, false);
            cf.AllowNullCells = anc;
            return l;
        }

        private List<PointF> TranslateNewInnerPath(List<PointF> newInnerPath, RectangleF rc, int breite)
        {
            List<PointF> lOut = new List<PointF>();

            for (int i = 0; i <= newInnerPath.Count - 1; i++)
                lOut.Add(new PointF(newInnerPath[i].X + rc.X - breite, newInnerPath[i].Y + rc.Y - breite));

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
    }
}