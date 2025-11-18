using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MorphologicalProcessing2.Algorithms
{
    public class Erode : IMorphologicalOperation
    {
        public int[,]? Kernel { get; set; }

        public Bitmap? KernelBmp { get; set; }
        public void Dispose()
        {
            if (this.KernelBmp != null)
            {
                this.KernelBmp.Dispose();
                this.KernelBmp = null;
            }
        }

        public bool RotateDilationKernels { get; set; }

        public BackgroundWorker? BGW { get; set; }

        public int Sleep { get; internal set; }

        public unsafe void ApplyGrayscale(Bitmap bmp)
        {
            using (Bitmap bC = (Bitmap)bmp.Clone())
            {
                BitmapData bmSrc = bC.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bC.Width, bC.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;

                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                int cnt = 0;
                double step = 100.0 / nHeight;

                Parallel.For(0, nHeight, (y, loopState) =>
                {
                    if (this.BGW != null && this.BGW.WorkerSupportsCancellation && this.BGW.CancellationPending)
                        loopState.Break();

                    if (this.Sleep != 0)
                        Thread.Sleep(this.Sleep);

                    byte* p = (byte*)bmData.Scan0;
                    byte* pSrc = (byte*)bmSrc.Scan0;

                    int pos = y * stride;

                    if (this.Kernel != null)
                        for (int x = 0; x < nWidth; x++)
                        {
                            p[pos] = p[pos + 1] = p[pos + 2] = bErode(bmSrc, new Point(x, y), this.Kernel);
                            p[pos + 3] = pSrc[pos + 3];

                            pos += 4;
                        }

                    Interlocked.Increment(ref cnt);

                    if (this.BGW != null && this.BGW.WorkerReportsProgress)
                        this.BGW.ReportProgress((int)Math.Min(step * cnt, 100));
                });

                bmp.UnlockBits(bmData);
                bC.UnlockBits(bmSrc);

                if (this.BGW != null && this.BGW.WorkerReportsProgress)
                    this.BGW.ReportProgress(100);
            }
        }

        public bool Setup(int width, int height)
        {
            this.Kernel = new int[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    this.Kernel[x, y] = 1;

            return true;
        }

        public bool SetupEx(int width, int height)
        {
            this.Kernel = new int[width, height];
            double radiusA = width / 2.0;
            double radiusB = height / 2.0;

            double cntrX = radiusA;
            double cntrY = radiusB;

            double numEx = 1.0;

            if (radiusA > 0 || radiusB > 0)
            {
                if (radiusA >= radiusB)
                    numEx = Math.Sqrt((radiusA * radiusA) - (radiusB * radiusB)) / radiusA;
                else
                    numEx = Math.Sqrt((radiusB * radiusB) - (radiusA * radiusA)) / radiusB;
            }

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double xAb = x - cntrX;
                    double yAb = y - cntrY;
                    double theta = Math.Atan2(yAb, xAb);

                    double radius = Math.Sqrt(xAb * xAb + yAb * yAb);
                    double rMax = 0.0;

                    rMax = radiusB / Math.Sqrt(1.0 - numEx * numEx * Math.Cos(theta) * Math.Cos(theta));
                    if (radiusA < radiusB)
                    {
                        double theta2 = Math.Atan2(xAb, yAb);
                        rMax = radiusA / Math.Sqrt(1.0 - numEx * numEx * Math.Cos(theta2) * Math.Cos(theta2));
                    }

                    if (radius <= rMax)
                        this.Kernel[x, y] = 1;
                }

            return true;
        }

        private unsafe byte bErode(BitmapData bmSrc, Point pt, int[,] krnl)
        {
            int stride = bmSrc.Stride;
            int nWidth = bmSrc.Width;
            int nHeight = bmSrc.Height;

            byte* pSrc = (byte*)bmSrc.Scan0;
            int rH = krnl.GetLength(1) / 2;
            int cH = krnl.GetLength(0) / 2;

            int rH2 = rH;
            if ((krnl.GetLength(1) & 0x01) != 1)
                rH2--;
            int cH2 = cH;
            if ((krnl.GetLength(0) & 0x01) != 1)
                cH2--;

            int x = pt.X;
            int y = pt.Y;

            byte b = 255;

            for (int r = -rH; r <= rH2; r++)
            {
                if (y + r >= 0 && y + r < nHeight)
                {
                    for (int c = -cH; c <= cH2; c++)
                    {
                        if (x + c >= 0 && x + c < nWidth)
                        {
                            if (krnl[c + cH, r + rH] == 1)
                                b = Math.Min(b, pSrc[(r + y) * stride + (c + x) * 4]);
                        }
                    }
                }
            }

            return b;
        }

        private unsafe byte bErode(BitmapData bmSrc, Point pt, int[,] krnl, int plane)
        {
            int stride = bmSrc.Stride;
            int nWidth = bmSrc.Width;
            int nHeight = bmSrc.Height;

            byte* pSrc = (byte*)bmSrc.Scan0;
            int rH = krnl.GetLength(1) / 2;
            int cH = krnl.GetLength(0) / 2;

            int rH2 = rH;
            if ((krnl.GetLength(1) & 0x01) != 1)
                rH2--;
            int cH2 = cH;
            if ((krnl.GetLength(0) & 0x01) != 1)
                cH2--;

            int x = pt.X;
            int y = pt.Y;

            byte b = 255;

            for (int r = -rH; r <= rH2; r++)
            {
                if (y + r >= 0 && y + r < nHeight)
                {
                    for (int c = -cH; c <= cH2; c++)
                    {
                        if (x + c >= 0 && x + c < nWidth)
                        {
                            if (krnl[c + cH, r + rH] == 1)
                                b = Math.Min(b, pSrc[(r + y) * stride + (c + x) * 4 + plane]);
                        }
                    }
                }
            }

            return b;
        }
    }
}
