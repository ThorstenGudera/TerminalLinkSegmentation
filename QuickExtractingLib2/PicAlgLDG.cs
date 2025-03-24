using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    public class PicAlgLDG : PicAlg
    {
        public BitmapData? BmpDataForValueComputation { get; private set; }

        public PicAlgLDG(BitmapData? bmpDataForValueComputation)
        {
            this.BmpDataForValueComputation = bmpDataForValueComputation;
        }

        public BitmapData? BmpDataColor { get; internal set; }
        public bool UseColVal { get; internal set; }

        internal unsafe override Tuple<double, bool> ComputeValueToNeighbor(BitmapData bmpDataForValueComputation,
            int address, int neighbor, int stride, double valL, double valM, double valG,
            int laplaceUpperThreshold, bool procNeighbor, int dist, double edgeWeight, double scale,
            bool useProcNeighbor, double valP, double valI, double valO, double valCl, double valCol)
        {
            if (double.IsInfinity(scale))
                scale = 1000000;

            if (address < 0 || address >= bmpDataForValueComputation.Height * stride ||
                neighbor < 0 || neighbor >= bmpDataForValueComputation.Height * stride)
                return Tuple.Create(100000.0, true);

            if (valL + valM + valG + valCl + valCol == 0.0)
                return Tuple.Create(100000.0, true);

            int x = address % stride;
            int y = address / stride;
            int x2 = neighbor % stride;
            int y2 = neighbor / stride;
            int xd = (x2 - x) / 4;
            int yd = y2 - y;

            int laplaceLowerThreshold = 255 - laplaceUpperThreshold;
            bool isEdge = false;

            byte* p = (byte*)bmpDataForValueComputation.Scan0;
            int length = stride * bmpDataForValueComputation.Height;

            //testing...
            double colVal = 1.0;

            if (this.UseColVal && (this.BmpDataColor != null))
            {
                if (address < 0 || address >= this.BmpDataColor.Height * this.BmpDataColor.Stride ||
                    neighbor < 0 || neighbor >= this.BmpDataColor.Height * this.BmpDataColor.Stride)
                    return Tuple.Create(100000.0, true);

                byte* p2 = (byte*)BmpDataColor.Scan0;
                int distB = Math.Abs((int)p2[address] - (int)p2[neighbor]);
                int distG = Math.Abs((int)p2[address + 1] - (int)p2[neighbor + 1]);
                int distR = Math.Abs((int)p2[address + 2] - (int)p2[neighbor + 2]);

                int max = Math.Max(distB, Math.Max(distG, distR));

                colVal = 2.0 - ((distB + distG + distR) / 3.0 / 127.0);
                colVal *= 1.0 - (max / 255.0);

                if ((1.0 - (max / 255.0)) <= edgeWeight)
                    isEdge = true;
            }

            double fL = 1d;

            if (useProcNeighbor && p[neighbor + 3] < length)
                fL = procNeighbor &&
                ((p[address + 3] > laplaceUpperThreshold && p[neighbor + 3] < laplaceLowerThreshold) ||
                 (p[address + 3] < laplaceLowerThreshold && p[neighbor + 3] > laplaceUpperThreshold)) &&
                 Math.Abs(p[neighbor + 3] - 127) < Math.Abs(p[address + 3] - 127) ? 0 : 1;
            else if (p[neighbor + 3] < length)
                fL = ((p[address + 3] > laplaceUpperThreshold && p[neighbor + 3] < laplaceLowerThreshold) ||
                 (p[address + 3] < laplaceLowerThreshold && p[neighbor + 3] > laplaceUpperThreshold)) ? 0 : 1;

            if (fL == 0)
                isEdge = true;

            double fG = 1.0;

            if (this.CostMaps != null && CostMaps.Ramps != null && CostMaps.Ramps.Count > 0 && this.CostMaps.CostMapGrad != null)
                fG = this.CostMaps.CostMapGrad[p[address + 2]];
            else
                fG = 1.0 - p[address + 2] / 255.0;

            if (fG <= edgeWeight)
                isEdge = true;

            //if (isEdge && y == 999 && Math.Floor(x / 4.0) > 893)
            //    x = x;

            int valAddressX = (int)p[address] - 127;
            int valAddressY = (int)p[address + 1] - 127;
            int valNeighborX = (int)p[neighbor] - 127;
            int valNeighborY = (int)(p[neighbor + 1]) - 127;

            valAddressX *= 2;
            valAddressY *= 2;
            valNeighborX *= 2;
            valNeighborY *= 2;

            double abs = Math.Sqrt(valAddressX * valAddressX + valAddressY * valAddressY);
            double dX = 0;
            double dY = 0;
            if (abs > 0)
            {
                dX = valAddressY / abs;
                dY = -valAddressX / abs;
            }

            double l1 = dX * xd + dY * yd;
            PointF l;

            double n = Math.Sqrt(xd * xd + yd * yd);

            if (l1 >= 0)
                l = new PointF((float)(xd / n), (float)(yd / n));
            else
                l = new PointF((float)(-xd / n), (float)(-yd / n));

            double abs2 = Math.Sqrt(valNeighborX * valNeighborX + valNeighborY * valNeighborY);
            double dX2 = 0;
            double dY2 = 0;

            if (abs2 > 0)
            {
                dX2 = valNeighborY / abs2;
                dY2 = -valNeighborX / abs2;
            }

            double dp = dX * l.X + dY * l.Y;
            double dq = l.X * dX2 + l.Y * dY2;

            double fD = (Math.Acos(dp) + Math.Acos(dq)) * 2 / (3 * Math.PI) * 4; //evtl without * 4...

            //test edgeClamp
            double gradCl = 1d;

            if (Math.Abs(neighbor - address) == 4)
                gradCl = Math.Abs(dY2 - dY); // (128.0 - Math.Abs((double)(p[address + 1] - 127))) / 127.0;
            else
                gradCl = Math.Abs(dX2 - dX); // (128.0 - Math.Abs((double)(p[address] - 127))) / 127.0;

            // later, after training
            if (this.CostMaps != null && this.CostMaps.Ramps != null && this.CostMaps.Ramps.Count > 3)
            {
                double fP = 0;
                double fI = 0;
                double fO = 0;

                if (this.GrayRepresentation != null && this.StrideGray > 0 && dX != 0 || dY != 0)
                {
                    if (this.CostMaps.CostMapEdge != null && this.GrayRepresentation != null)
                    {
                        fP = this.CostMaps.CostMapEdge[this.GrayRepresentation[address / 4]];

                        int xx = x / 4;
                        int xxx = (int)(dist * dX);
                        int yyy = (int)(dist * dY);

                        double fIX = (xx + xxx);
                        double fIY = (y + yyy) * this.StrideGray;

                        if (this.CostMaps.CostMapIn != null && this.CostMaps.CostMapOut != null)
                        {
                            fI = (int)(fIY + fIX) >= 0 && (int)(fIY + fIX) < this.GrayRepresentation.Length ? this.CostMaps.CostMapIn[this.GrayRepresentation[(int)(fIY + fIX)]] : 0;

                            // My.Application.Log.WriteEntry(DateTime.Now.ToString() & xx.ToString & " new " & (CInt(fIY + fIX) Mod StrideGray).ToString() & " - " & y.ToString() & " new " & (CInt(fIY + fIX) \ StrideGray).ToString())

                            double fOX = (xx - xxx);
                            double fOY = (y - yyy) * this.StrideGray;
                            int zz = (int)(fOY + fOX);
                            fO = (int)(fOY + fOX) >= 0 && (int)(fOY + fOX) < this.GrayRepresentation.Length ? this.CostMaps.CostMapOut[this.GrayRepresentation[(int)(fOY + fOX)]] : 0;
                        }
                    }
                }
                return new Tuple<double, bool>((double)((valL * fL + valG * fD + valM * fG + valP * fP + valI * fI + valO * fO + valCl * gradCl + valCol * colVal) * 3 * scale), isEdge);
            }

            return new Tuple<double, bool>((double)((valL * fL + valG * fD + valM * fG + valCl * gradCl + valCol * colVal) * 3 * scale), isEdge);
        }

        internal override byte[] InitBmpDataForValueComputation(BitmapData data, int stride, bool doR, bool doG, bool doB)
        {
            int l = data.Height * stride;
            if (AvailMem.AvailMem.checkAvailRam(l * 2L))
            {
                byte[] f = new byte[l];
                byte[] ff = new byte[l / 4];

                byte[] p = new byte[l];
                Marshal.Copy(data.Scan0, p, 0, p.Length);

                Grayscale(p, ff, stride, doR, doG, doB);
                GetDerivatives(ff, f, stride); // channels 1 and 2
                GetMagnitude(f); // channel 3
                GetSecondDerivative(ff, f, stride); // channel 4

                return f;
            }
            else
                return new byte[0];
        }

        private void Grayscale(byte[] p, byte[] ff, int stride, bool doR, bool doG, bool doB)
        {
            if (doR && doG && doB)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = System.Convert.ToByte(Math.Min(System.Convert.ToDouble(b) * 0.11 + System.Convert.ToDouble(g) * 0.59 + System.Convert.ToDouble(r) * 0.3, 255));
                });
            }

            if (doR && !doG && !doB)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = (byte)r; // System.Convert.ToByte(Math.Min(System.Convert.ToDouble(r) * 0.11 + System.Convert.ToDouble(r) * 0.59 + System.Convert.ToDouble(r) * 0.3, 255));
                });
            }

            if (doG && !doR && !doB)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = (byte)g; // System.Convert.ToByte(Math.Min(System.Convert.ToDouble(g) * 0.11 + System.Convert.ToDouble(g) * 0.59 + System.Convert.ToDouble(g) * 0.3, 255));
                });
            }

            if (doB && !doR && !doG)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = (byte)b; // System.Convert.ToByte(Math.Min(System.Convert.ToDouble(b) * 0.11 + System.Convert.ToDouble(b) * 0.59 + System.Convert.ToDouble(b) * 0.3, 255));
                });
            }

            if (doR && doG && !doB)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = System.Convert.ToByte(Math.Min(System.Convert.ToDouble(((r + g) / (double)2)) * 0.11 + System.Convert.ToDouble(g) * 0.59 + System.Convert.ToDouble(r) * 0.3, 255));
                });
            }

            if (doR && doB && !doG)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = System.Convert.ToByte(Math.Min(System.Convert.ToDouble(b) * 0.11 + System.Convert.ToDouble(((r + b) / (double)2)) * 0.59 + System.Convert.ToDouble(r) * 0.3, 255));
                });
            }

            if (doG && doB && !doR)
            {
                Parallel.For(0, ff.Length - 1, i =>
                {
                    int ii = i * 4;
                    int b = p[ii];
                    int g = p[ii + 1];
                    int r = p[ii + 2];

                    ff[i] = System.Convert.ToByte(Math.Min(System.Convert.ToDouble(b) * 0.11 + System.Convert.ToDouble(g) * 0.59 + System.Convert.ToDouble(((b + g) / (double)2)) * 0.3, 255));
                });
            }

            this.GrayRepresentation = ff;
            this.StrideGray = stride / 4;
        }

        private void GetSecondDerivative(byte[] p, byte[] f, int stride)
        {
            try
            {
                ConvMatrix mx = new ConvMatrix();
                mx.SetAll(0);
                mx.TopLeft = 1;
                mx.TopMid = 2;
                mx.TopRight = 1;
                mx.MidLeft = 2;
                mx.Pixel = -12;
                mx.MidRight = 2;
                mx.BottomLeft = 1;
                mx.BottomMid = 2;
                mx.BottomRight = 1;
                mx.Factor = 1;
                mx.Offset = 127;
                int factor22 = 1;
                int factor222 = 1;
                int eins = 0;
                int zwei = 0;
                int drei = 0;
                int vier = 0;
                int fuenf = 0;
                int sechs = 0;
                int sieben = 0;
                int acht = 0;
                int neun = 0;

                eins = 3;
                drei = 3;
                zwei = 4;
                vier = 4;
                sieben = 3;
                neun = 3;
                acht = 4;
                sechs = 4;

                bool a = Gray_Conv3x3_IntoColorPlane(mx, factor22, factor222, eins, zwei, drei, vier, fuenf, sechs, sieben, acht, neun, p, f, 3, stride / 4, stride);
            }
            catch
            {
            }
        }

        private void GetMagnitude(byte[] f)
        {
            Parallel.For(0, f.Length / 4, i =>
            {
                int j = i * 4;
                f[j + 2] = (byte)Math.Max(Math.Min(Math.Sqrt(((int)f[j] - 127) * ((int)f[j] - 127) * 4 + ((int)f[j + 1] - 127) * ((int)f[j + 1] - 127) * 4) / Math.Sqrt(2.0) - 1.0, 255), 0);

                //f[j + 2] = (byte)(Math.Min(Math.Sqrt(((int)f[j] - 127) * ((int)f[j] - 127) * 4 + ((int)f[j + 1] - 127) * ((int)f[j + 1] - 127) * 4), 255));
            });
        }

        private void GetDerivatives(byte[] p, byte[] f, int stride)
        {
            EdgeDetection_OptimizedSobel_Horz(p, f, stride);
            EdgeDetection_OptimizedSobel_Vert(p, f, stride);
        }

        internal static bool Gray_Conv3x3_IntoColorPlane(ConvMatrix mx, double Factor22, double Factor222,
            int eins, int zwei, int drei, int vier, int fuenf, int sechs, int sieben, int acht, int neun,
            byte[] p, byte[] f, int planeWrite, int stride, int strideWrite)
        {
            if (0 == mx.Factor)
                return false;
            if (0.0 == Factor22)
                return false;
            if (0.0 == Factor222)
                return false;

            if (planeWrite < 0 || planeWrite > 3)
                return false;

            try
            {
                int stride2 = stride * 2;

                int pos = 0;

                int nWidth = stride - 2;
                int nHeight = p.Length / stride - 2;

                int nPixel;

                nPixel = (int)((((p[pos] * (mx.Pixel + eins)) + (p[pos + 1] * mx.MidRight) + (p[pos + stride] * mx.BottomMid) + (p[pos + 1 + stride] * mx.BottomRight)) / (double)(mx.Factor * Factor22)) + mx.Offset);

                if (nPixel < 0)
                    nPixel = 0;
                if (nPixel > 255)
                    nPixel = 255;
                f[pos * 4 + planeWrite] = (byte)(nPixel);

                for (int x = 0; x < nWidth; x++)
                {
                    nPixel = (int)((((p[pos] * mx.MidLeft) + (p[pos + 1] * (mx.Pixel + zwei)) + (p[pos + 2] * mx.MidRight) + (p[pos + stride] * mx.BottomLeft) + (p[pos + 1 + stride] * mx.BottomMid) + (p[pos + 2 + stride] * mx.BottomRight)) / (double)(mx.Factor * Factor222)) + mx.Offset);

                    if (nPixel < 0)
                        nPixel = 0;
                    if (nPixel > 255)
                        nPixel = 255;
                    f[pos * 4 + 4 + planeWrite] = (byte)(nPixel);

                    pos += 1;
                }

                nPixel = (int)((((p[pos] * mx.MidLeft) + (p[pos + 1] * (mx.Pixel + drei)) + (p[pos + stride] * mx.BottomLeft) + (p[pos + 1 + stride] * mx.BottomMid)) / (double)(mx.Factor * Factor22)) + mx.Offset);

                if (nPixel < 0)
                    nPixel = 0;
                if (nPixel > 255)
                    nPixel = 255;
                f[pos * 4 + 4 + planeWrite] = (byte)(nPixel);

                pos = 0;

                int nHeight1 = nHeight;

                Parallel.For(0, nHeight1, y =>
                {
                    // For y As Integer = 0 To nHeight - 1
                    int nPixel2 = 0;
                    int pos2 = y * stride;
                    ConvMatrix mx2 = (ConvMatrix)mx.Clone();

                    // first pixel
                    nPixel2 = (int)((((p[pos2] * mx2.TopMid) + (p[pos2 + 1] * mx2.TopRight) + (p[pos2 + stride] * (mx2.Pixel + vier)) + (p[pos2 + 1 + stride] * mx2.MidRight) + (p[pos2 + stride2] * mx2.BottomMid) + (p[pos2 + 1 + stride2] * mx2.BottomRight)) / (double)(mx2.Factor * Factor222)) + mx2.Offset);

                    if (nPixel2 < 0)
                        nPixel2 = 0;
                    if (nPixel2 > 255)
                        nPixel2 = 255;
                    f[pos2 * 4 + strideWrite + planeWrite] = (byte)(nPixel2);

                    for (int x = 0; x < nWidth; x++)
                    {
                        nPixel2 = (int)((((p[pos2] * mx2.TopLeft) + (p[pos2 + 1] * mx2.TopMid) + (p[pos2 + 2] * mx2.TopRight) + (p[pos2 + stride] * mx2.MidLeft) + (p[pos2 + 1 + stride] * (mx2.Pixel + fuenf)) + (p[pos2 + 2 + stride] * mx2.MidRight) + (p[pos2 + stride2] * mx2.BottomLeft) + (p[pos2 + 1 + stride2] * mx2.BottomMid) + (p[pos2 + 2 + stride2] * mx2.BottomRight)) / (double)mx2.Factor) + mx2.Offset);

                        if (nPixel2 < 0)
                            nPixel2 = 0;
                        if (nPixel2 > 255)
                            nPixel2 = 255;

                        f[pos2 * 4 + 4 + strideWrite + planeWrite] = (byte)(nPixel2);

                        pos2 += 1;
                    }

                    // last pixel
                    nPixel2 = (int)((((p[pos2] * mx2.TopLeft) + (p[pos2 + 1] * mx2.TopMid) + (p[pos2 + stride] * mx2.MidLeft) + (p[pos2 + 1 + stride] * (mx2.Pixel + sechs)) + (p[pos2 + stride2] * mx2.BottomLeft) + (p[pos2 + 1 + stride2] * mx2.BottomMid)) / (double)(mx2.Factor * Factor222)) + mx2.Offset);

                    if (nPixel2 < 0)
                        nPixel2 = 0;
                    if (nPixel2 > 255)
                        nPixel2 = 255;
                    f[pos2 * 4 + 4 + strideWrite + planeWrite] = (byte)(nPixel2);

                    pos2 += 2;
                });

                pos = p.Length - stride2;

                // last line
                nPixel = (int)((((p[pos] * mx.TopMid) + (p[pos + 1] * mx.TopRight) + (p[pos + stride] * (mx.Pixel + sieben)) + (p[pos + 1 + stride] * mx.MidRight)) / (double)(mx.Factor * Factor22)) + mx.Offset);

                if (nPixel < 0)
                    nPixel = 0;
                if (nPixel > 255)
                    nPixel = 255;
                f[pos * 4 + strideWrite + planeWrite] = (byte)(nPixel);

                for (int x = 0; x < nWidth; x++)
                {
                    nPixel = (int)((((p[pos] * mx.TopLeft) + (p[pos + 1] * mx.TopMid) + (p[pos + 2] * mx.TopRight) + (p[pos + stride] * mx.MidLeft) + (p[pos + 1 + stride] * (mx.Pixel + acht)) + (p[pos + 2 + stride] * mx.MidRight)) / (double)(mx.Factor * Factor222)) + mx.Offset);

                    if (nPixel < 0)
                        nPixel = 0;
                    if (nPixel > 255)
                        nPixel = 255;
                    f[pos * 4 + 4 + strideWrite + planeWrite] = (byte)(nPixel);

                    pos += 1;
                }

                nPixel = (int)((((p[pos] * mx.TopLeft) + (p[pos + 1] * mx.TopMid) + (p[pos + stride] * mx.MidLeft) + (p[pos + 1 + stride] * (mx.Pixel + neun))) / (double)(mx.Factor * Factor22)) + mx.Offset);

                if (nPixel < 0)
                    nPixel = 0;
                if (nPixel > 255)
                    nPixel = 255;

                f[pos * 4 + 4 + strideWrite + planeWrite] = (byte)(nPixel);

                return true;
            }
            catch
            {
            }

            return false;
        }

        public static bool EdgeDetection_OptimizedSobel_Horz(byte[] p, byte[] f, int stride)
        {
            try
            {
                ConvMatrix mx = new ConvMatrix();
                mx.SetAll(0);
                mx.TopLeft = 3;
                mx.MidLeft = 10;
                mx.BottomLeft = 3;
                mx.Pixel = 0;
                mx.TopRight = -3;
                mx.MidRight = -10;
                mx.BottomRight = -3;
                mx.Factor = 1;
                mx.Offset = 127;
                int factor22 = 1;
                int factor222 = 1;
                int eins = 0;
                int zwei = 0;
                int drei = 0;
                int vier = 0;
                int fuenf = 0;
                int sechs = 0;
                int sieben = 0;
                int acht = 0;
                int neun = 0;

                eins = 13;
                sieben = 13;
                vier = 16;
                drei = -13;
                neun = -13;
                sechs = -16;

                bool a = Gray_Conv3x3_IntoColorPlane(mx, factor22, factor222, eins, zwei, drei, vier, fuenf, sechs, sieben, acht, neun, p, f, 0, stride / 4, stride);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool EdgeDetection_OptimizedSobel_Vert(byte[] p, byte[] f, int stride)
        {
            try
            {
                ConvMatrix mx = new ConvMatrix();
                mx.SetAll(0);
                mx.TopLeft = 3;
                mx.TopMid = 10;
                mx.TopRight = 3;
                mx.Pixel = 0;
                mx.BottomLeft = -3;
                mx.BottomMid = -10;
                mx.BottomRight = -3;
                mx.Factor = 1;
                mx.Offset = 127;
                int factor22 = 1;
                int factor222 = 1;
                int eins = 0;
                int zwei = 0;
                int drei = 0;
                int vier = 0;
                int fuenf = 0;
                int sechs = 0;
                int sieben = 0;
                int acht = 0;
                int neun = 0;

                eins = 13;
                drei = 13;
                zwei = 16;
                sieben = -13;
                neun = -13;
                acht = -16;

                bool a = Gray_Conv3x3_IntoColorPlane(mx, factor22, factor222, eins, zwei, drei, vier, fuenf, sechs, sieben, acht, neun, p, f, 1, stride / 4, stride);

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.GrayRepresentation = null;
            base.Dispose(disposing);
        }
    }
}