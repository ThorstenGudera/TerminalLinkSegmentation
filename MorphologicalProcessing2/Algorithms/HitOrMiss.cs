using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphologicalProcessing2.Algorithms
{
    internal class HitOrMiss : IMorphologicalOperation
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

        public unsafe void ApplyGrayscale(Bitmap bmp)
        {
            using (Bitmap bC = (Bitmap)bmp.Clone())
            {
                BitmapData bmSrc = bC.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bC.Width, bC.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;

                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                if (this.Kernel != null)
                    Parallel.For(0, nHeight, (y, loopState) =>
                    {
                        if (this.BGW != null && this.BGW.WorkerSupportsCancellation && this.BGW.CancellationPending)
                            loopState.Break();

                        byte* p = (byte*)bmData.Scan0;
                        byte* pSrc = (byte*)bmSrc.Scan0;

                        int pos = y * stride;

                        for (int x = 0; x < nWidth; x++)
                        {
                            p[pos] = p[pos + 1] = p[pos + 2] = (byte)((int)bHOM(bmSrc, new Point(x, y), this.Kernel) * 255);
                            p[pos + 3] = pSrc[pos + 3];

                            pos += 4;
                        }
                    });

                bmp.UnlockBits(bmData);
                bC.UnlockBits(bmSrc);
            }
        }

        internal unsafe byte bHOM(BitmapData bmSrc, Point pt, int[,] krnl)
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

            byte b = 0;

            for (int r = -rH; r <= rH2; r++)
            {
                if (y + r >= 0 && y + r < nHeight)
                {
                    for (int c = -cH; c <= cH2; c++)
                    {
                        if (x + c >= 0 && x + c < nWidth)
                        {
                            double v = (double)pSrc[(r + y) * stride + (c + x) * 4] / 255.0;
                            if (krnl[c + cH, r + rH] == 1)
                            {
                                if (v > 0.75)
                                    b = 1;
                                else
                                    return 0;
                            }
                            else if (krnl[c + cH, r + rH] == 0)
                            {
                                if (v < 0.25)
                                    b = 1;
                                else
                                    return 0;
                            }
                        }
                    }
                }
            }

            return b;
        }

        internal unsafe byte bHOM(BitmapData bmSrc, Point pt, int[,] krnl, int plane)
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

            byte b = 0;

            for (int r = -rH; r <= rH2; r++)
            {
                if (y + r >= 0 && y + r < nHeight)
                {
                    for (int c = -cH; c <= cH2; c++)
                    {
                        if (x + c >= 0 && x + c < nWidth)
                        {
                            double v = (double)pSrc[(r + y) * stride + (c + x) * 4 + plane] / 255.0;
                            if (krnl[c + cH, r + rH] == 1)
                            {
                                if (v > 0.75)
                                    b = 1;
                                else
                                    return 0;
                            }
                            else if (krnl[c + cH, r + rH] == 0)
                            {
                                if (v < 0.25)
                                    b = 1;
                                else
                                    return 0;
                            }
                        }
                    }
                }
            }

            return b;
        }

        public bool Setup(int width, int height)
        {
            if (this.KernelBmp != null)
            {
                this.Kernel = ReadBmp();
                return true;
            }
            return false;
        }

        public bool SetupEx(int width, int height)
        {
            return Setup(width, height);
        }

        private int[,]? ReadBmp()
        {
            if (this.KernelBmp != null)
            {
                int[,] krnl = new int[this.KernelBmp.Width, this.KernelBmp.Height];

                for (int y = 0; y < this.KernelBmp.Height; y++)
                {
                    for (int x = 0; x < this.KernelBmp.Width; x++)
                    {
                        krnl[x, y] = (this.KernelBmp.GetPixel(x, y).GetBrightness() > 0.75) ? 1 : (this.KernelBmp.GetPixel(x, y).GetBrightness() < 0.25) ? 0 : -1;
                    }
                }

                return krnl;
            }
            return null;
        }

        private int[,]? ReadBmp(int plane)
        {
            if (this.KernelBmp != null)
            {
                int[,] krnl = new int[this.KernelBmp.Width, this.KernelBmp.Height];

                for (int y = 0; y < this.KernelBmp.Height; y++)
                {
                    for (int x = 0; x < this.KernelBmp.Width; x++)
                    {
                        if (plane == 0)
                            krnl[x, y] = (this.KernelBmp.GetPixel(x, y).B > 191) ? 1 : (this.KernelBmp.GetPixel(x, y).B < 64) ? 0 : -1;
                        else if (plane == 1)
                            krnl[x, y] = (this.KernelBmp.GetPixel(x, y).G > 191) ? 1 : (this.KernelBmp.GetPixel(x, y).G < 64) ? 0 : -1;
                        else if (plane == 2)
                            krnl[x, y] = (this.KernelBmp.GetPixel(x, y).R > 191) ? 1 : (this.KernelBmp.GetPixel(x, y).R < 64) ? 0 : -1;
                    }
                }

                return krnl;
            }
            return null;
        }
    }
}
