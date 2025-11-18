using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MorphologicalProcessing2.Algorithms
{
    public class ConvexHull : IMorphologicalOperation
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
            using (Bitmap bC = (Bitmap)bmp.Clone(), bCur = (Bitmap)bmp.Clone())
            {
                Rectangle rc = ScanForPicBW(bmp);
                int l = rc.X;
                int t = rc.Y;
                int r = bmp.Width - (rc.X + rc.Width);
                int b = bmp.Height - (rc.Y + rc.Height);

                BitmapData bmSrc = bC.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bC.Width, bC.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmCur = bCur.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;

                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                List<Bitmap> innerResults = new List<Bitmap>();

                for (int i = 0; i < 4; i++)
                {
                    bool isDifferent = true;
                    this.Kernel = GetSE(i);
                    int cnt = 0;

                    if (this.BGW != null && this.BGW.WorkerSupportsCancellation && this.BGW.CancellationPending)
                        break;

                    while (isDifferent)
                    {
                        if (cnt > 0)
                            SetEqual(bmSrc, bmCur, nWidth, nHeight, stride); //cur and src = k-1

                        Parallel.For(t, nHeight - b, (y, loopState) =>
                        {
                            if (this.BGW != null && this.BGW.WorkerSupportsCancellation && this.BGW.CancellationPending)
                                loopState.Break();

                            byte* pCur = (byte*)bmCur.Scan0;
                            byte* pSrc = (byte*)bmSrc.Scan0;

                            int pos = y * stride + l * 4;

                            for (int x = l; x < nWidth - r; x++)
                            {
                                pCur[pos] = pCur[pos + 1] = pCur[pos + 2] = (byte)(HOM(bmSrc, new Point(x, y), this.Kernel) * 255);
                                pCur[pos + 3] = pSrc[pos + 3];

                                pos += 4;
                            }
                        });

                        GetUnion(bmCur, bmSrc, nWidth, nHeight, stride); //now cur = k, src = k-1

                        if (this.BGW != null && this.BGW.WorkerSupportsCancellation && this.BGW.CancellationPending)
                            break;

                        isDifferent = IsDifferent(bmSrc, bmCur, nWidth, nHeight, stride);

                        cnt++;
                    }

                    innerResults.Add(GetCopy(bmCur, nWidth, nHeight, stride));
                }

                GetResultingUnion(bmData, innerResults, nWidth, nHeight, stride);

                bmp.UnlockBits(bmData);
                bC.UnlockBits(bmSrc);
                bCur.UnlockBits(bmCur);
            }
        }

        private Rectangle ScanForPicBW(Bitmap bmp)
        {
            BitmapData? bmData = null;
            Rectangle rectangle = new Rectangle(-1, -1, -1, -1);

            try
            {
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int scanline = bmData.Stride;

                System.IntPtr Scan0 = bmData.Scan0;

                Point top = new Point(), left = new Point(), right = new Point(), bottom = new Point();
                bool complete = false;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if (p[0] > 191)
                            {
                                top = new Point(x, y);
                                complete = true;
                                break;
                            }

                            p += 4;
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int y = bmp.Height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if (p[x * 4 + y * scanline] > 191)
                            {
                                bottom = new Point(x + 1, y + 1);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            if (p[x * 4 + y * scanline] > 191)
                            {
                                left = new Point(x, y);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int x = bmp.Width - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            if (p[x * 4 + y * scanline] > 191)
                            {
                                right = new Point(x + 1, y + 1);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }
                }

                bmp.UnlockBits(bmData);

                rectangle = new Rectangle(left.X, top.Y, right.X - left.X, bottom.Y - top.Y);
            }
            catch
            {
                try
                {
                    if(bmData != null)
                        bmp.UnlockBits(bmData);
                }
                catch
                {

                }
            }

            return rectangle;
        }

        private Rectangle ScanForPicBW(BitmapData bmData, int nWidth, int nHeight, int stride, int plane)
        {
            Rectangle rectangle = new Rectangle(-1, -1, -1, -1);

            try
            {
                int scanline = bmData.Stride;

                System.IntPtr Scan0 = bmData.Scan0;

                Point top = new Point(), left = new Point(), right = new Point(), bottom = new Point();
                bool complete = false;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    for (int y = 0; y < nHeight; y++)
                    {
                        for (int x = 0; x < nWidth; x++)
                        {
                            if (p[0] > 191)
                            {
                                top = new Point(x, y);
                                complete = true;
                                break;
                            }

                            p += 4;
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int y = nHeight - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < nWidth; x++)
                        {
                            if (p[x * 4 + y * scanline] > 191)
                            {
                                bottom = new Point(x + 1, y + 1);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int x = 0; x < nWidth; x++)
                    {
                        for (int y = 0; y < nHeight; y++)
                        {
                            if (p[x * 4 + y * scanline] > 191)
                            {
                                left = new Point(x, y);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    p = (byte*)(void*)Scan0;
                    complete = false;

                    for (int x = nWidth - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < nHeight; y++)
                        {
                            if (p[x * 4 + y * scanline] > 191)
                            {
                                right = new Point(x + 1, y + 1);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }
                }

                rectangle = new Rectangle(left.X, top.Y, right.X - left.X, bottom.Y - top.Y);
            }
            catch
            {

            }

            return rectangle;
        }

        private void GetResultingUnion(BitmapData bmData, List<Bitmap> innerResults, int nWidth, int nHeight, int stride)
        {
            if (innerResults != null)
                for (int i = 0; i < innerResults.Count; i++)
                {
                    BitmapData bmCur = innerResults[i].LockBits(new Rectangle(0, 0, nWidth, nHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    GetUnion(bmData, bmCur, nWidth, nHeight, stride);
                    innerResults[i].UnlockBits(bmCur);
                }
        }

        private void GetResultingUnion(BitmapData bmData, List<Bitmap> innerResults, int nWidth, int nHeight, int stride, int plane)
        {
            if (innerResults != null)
                for (int i = 0; i < innerResults.Count; i++)
                {
                    BitmapData bmCur = innerResults[i].LockBits(new Rectangle(0, 0, nWidth, nHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    GetUnion(bmData, bmCur, nWidth, nHeight, stride, plane);
                    innerResults[i].UnlockBits(bmCur);
                }
        }

        private unsafe Bitmap GetCopy(BitmapData bmSrc, int nWidth, int nHeight, int stride)
        {
            Bitmap bRes = new Bitmap(nWidth, nHeight);

            BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, nWidth, nHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Parallel.For(0, nHeight, y =>
            {
                byte* pCur = (byte*)bmData.Scan0;
                byte* pSrc = (byte*)bmSrc.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    pCur[pos] = pCur[pos + 1] = pCur[pos + 2] = pSrc[pos];
                    pCur[pos + 3] = pSrc[pos + 3];

                    pos += 4;
                }
            });

            bRes.UnlockBits(bmData);

            return bRes;
        }

        private unsafe Bitmap GetCopy(BitmapData bmSrc, int nWidth, int nHeight, int stride, int plane)
        {
            Bitmap bRes = new Bitmap(nWidth, nHeight);

            BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, nWidth, nHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Parallel.For(0, nHeight, y =>
            {
                byte* pCur = (byte*)bmData.Scan0;
                byte* pSrc = (byte*)bmSrc.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    pCur[pos + plane] = pSrc[pos + plane];
                    pCur[pos + 3] = pSrc[pos + 3];

                    pos += 4;
                }
            });

            bRes.UnlockBits(bmData);

            return bRes;
        }

        private unsafe void SetEqual(BitmapData bmTo, BitmapData bmFrom, int nWidth, int nHeight, int stride)
        {
            Parallel.For(0, nHeight, y =>
            {
                byte* pTo = (byte*)bmTo.Scan0;
                byte* pFrom = (byte*)bmFrom.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    pTo[pos] = pTo[pos + 1] = pTo[pos + 2] = pFrom[pos];
                    pTo[pos + 3] = pFrom[pos + 3];

                    pos += 4;
                }
            });
        }

        private unsafe void SetEqual(BitmapData bmTo, BitmapData bmFrom, int nWidth, int nHeight, int stride, int plane)
        {
            Parallel.For(0, nHeight, y =>
            {
                byte* pTo = (byte*)bmTo.Scan0;
                byte* pFrom = (byte*)bmFrom.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    pTo[pos + plane] = pFrom[pos + plane];
                    pTo[pos + 3] = pFrom[pos + 3];

                    pos += 4;
                }
            });
        }

        private byte HOM(BitmapData bmSrc, Point point, int[,] kernel)
        {
            HitOrMiss hom = new HitOrMiss();
            //hom.Kernel = kernel;
            return hom.bHOM(bmSrc, point, kernel);
        }

        private byte HOM(BitmapData bmSrc, Point point, int[,] kernel, int plane)
        {
            HitOrMiss hom = new HitOrMiss();
            //hom.Kernel = kernel;
            return hom.bHOM(bmSrc, point, kernel, plane);
        }

        private unsafe bool IsDifferent(BitmapData bmSrc, BitmapData bmCur, int nWidth, int nHeight, int stride)
        {
            bool found = false;

            Parallel.For(0, nHeight, (y, loopState) =>
            {
                byte* pCur = (byte*)bmCur.Scan0;
                byte* pSrc = (byte*)bmSrc.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    if (pCur[pos] != pSrc[pos])
                    {
                        found = true;
                        break;
                    }
                    pos += 4;
                }

                if (found)
                    loopState.Break();
            });

            return found;
        }

        private unsafe bool IsDifferent(BitmapData bmSrc, BitmapData bmCur, int nWidth, int nHeight, int stride, int plane)
        {
            bool found = false;

            Parallel.For(0, nHeight, (y, loopState) =>
            {
                byte* pCur = (byte*)bmCur.Scan0;
                byte* pSrc = (byte*)bmSrc.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    if (pCur[pos + plane] != pSrc[pos + plane])
                    {
                        found = true;
                        break;
                    }
                    pos += 4;
                }

                if (found)
                    loopState.Break();
            });

            return found;
        }

        private unsafe void GetUnion(BitmapData bmTo, BitmapData bmFrom, int nWidth, int nHeight, int stride)
        {
            Parallel.For(0, nHeight, y =>
            {
                byte* pCur = (byte*)bmFrom.Scan0;
                byte* pSrc = (byte*)bmTo.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    pSrc[pos] = pSrc[pos + 1] = pSrc[pos + 2] = (byte)Math.Max(Math.Min((int)pSrc[pos] + pCur[pos], 255), 0);

                    pos += 4;
                }
            });
        }

        private unsafe void GetUnion(BitmapData bmTo, BitmapData bmFrom, int nWidth, int nHeight, int stride, int plane)
        {
            Parallel.For(0, nHeight, y =>
            {
                byte* pCur = (byte*)bmFrom.Scan0;
                byte* pSrc = (byte*)bmTo.Scan0;

                int pos = y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    pSrc[pos + plane] = (byte)Math.Max(Math.Min((int)pSrc[pos + plane] + pCur[pos + plane], 255), 0);

                    pos += 4;
                }
            });
        }

        private int[,] GetSE(int i)
        {
            int[,] res = new int[3, 3];

            switch (i)
            {
                case 0:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            if (x == 0)
                                res[x, y] = 1;
                            else if (x == 1 && y == 1)
                                res[x, y] = 0;
                            else
                                res[x, y] = -1;
                        }
                    }
                    break;
                case 1:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            if (y == 0)
                                res[x, y] = 1;
                            else if (x == 1 && y == 1)
                                res[x, y] = 0;
                            else
                                res[x, y] = -1;
                        }
                    }
                    break;
                case 2:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            if (x == 2)
                                res[x, y] = 1;
                            else if (x == 1 && y == 1)
                                res[x, y] = 0;
                            else
                                res[x, y] = -1;
                        }
                    }
                    break;
                case 3:
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            if (y == 2)
                                res[x, y] = 1;
                            else if (x == 1 && y == 1)
                                res[x, y] = 0;
                            else
                                res[x, y] = -1;
                        }
                    }
                    break;
                default:
                    break;
            }

            return res;
        }

        public bool Setup(int width, int height)
        {
            return true;
        }

        public bool SetupEx(int width, int height)
        {
            return true;
        }
    }
}
