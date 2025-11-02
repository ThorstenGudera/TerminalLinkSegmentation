using ChainCodeFinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ConvolutionLib;
using System.Drawing.Imaging;

namespace AvoidAGrabCutEasy.ProcOutline
{
    public class Fipbmp
    {
        public event EventHandler<ConvolutionLib.ProgressEventArgs>? ProgressPlus;

        public static Bitmap? RemOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
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

        public static Bitmap? ExtOutline(Bitmap bmp, Bitmap bOrig, int breite, System.ComponentModel.BackgroundWorker? bgw)
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

                    ChainFinder cf2 = new ChainFinder();
                    cf2.AllowNullCells = true;

                    List<ChainCode> fList2 = cf2.GetOutline(b, 0, false, 0, false);

                    using (Graphics gx = Graphics.FromImage(b))
                    {
                        gx.Clear(Color.Transparent);

                        foreach (ChainCode c in fList2)
                        {
                            using (GraphicsPath gP = new GraphicsPath())
                            {
                                PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                gP.AddLines(pts);
                                gP.CloseAllFigures();

                                using (TextureBrush tb = new TextureBrush(bOrig))
                                {
                                    gx.FillPath(tb, gP);
                                    using (Pen p = new Pen(tb, 1))
                                        gx.DrawPath(p, gP);
                                }
                            }
                        }
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

        public void SmoothByAveragingA(Bitmap bWork, int krnl, BackgroundWorker? bgw)
        {
            double[,] KernelBB = new double[krnl - 1 + 1, krnl - 1 + 1];

            for (int y = 0; y <= KernelBB.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= KernelBB.GetLength(0) - 1; x++)
                    KernelBB[x, y] = 1.0;
            }

            Convolution conv = new Convolution();
            conv.ProgressPlus += conv_ProgressPlus;
            conv.CancelLoops = false;

            // for the edges of the image
            double[,] AddValsBB = conv.CalculateStandardAddVals(KernelBB);

            ConvolutionLib.ProgressEventArgs? pe = new ConvolutionLib.ProgressEventArgs(bWork.Height, 20);
            // go
            bool b = conv.Convolve_par(bWork, KernelBB, AddValsBB, 0, 255, true, false, pe, bgw);
            pe = null;

            conv.ProgressPlus -= conv_ProgressPlus;
        }

        private void conv_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            ProgressPlus?.Invoke(sender, e);
        }

        public unsafe void ReplaceColors(Bitmap? bmp, int nAlpha, int nRed, int nGreen, int nBlue, int tolerance, int zAlpha, int zRed, int zGreen, int zBlue)
        {
            BitmapData? bmData = null;
            if (bmp != null && AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                try
                {
                    bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;

                    int nWidth = bmp.Width;
                    int nHeight = bmp.Height;

                    //byte* p = (byte*)bmData.Scan0;// new byte[(bmData.Stride * bmData.Height)];

                    Parallel.For(0, nHeight, y =>
                    {
                        byte* p = (byte*)bmData.Scan0;
                        // for (int y = 0; y < bmp.Height; y++)
                        int pos = 0;
                        pos += y * stride;

                        for (int x = 0; x <= nWidth - 1; x++)
                        {
                            if ((p[pos + 3] == 0 && zAlpha == 0) ||
                                ((p[pos + 3] > (zAlpha - tolerance)) && (p[pos + 3] < (zAlpha + tolerance)) &&
                                (p[pos + 2] > (zRed - tolerance)) && (p[pos + 2] < (zRed + tolerance)) &&
                                (p[pos + 1] > (zGreen - tolerance)) && (p[pos + 1] < (zGreen + tolerance)) &&
                                (p[pos] > (zBlue - tolerance)) && (p[pos] < (zBlue + tolerance))))
                            {
                                int value = nAlpha + System.Convert.ToInt32(p[pos + 3]) - zAlpha;
                                if (value < 0)
                                    value = 0;
                                if (value > 255)
                                    value = 255;
                                p[pos + 3] = System.Convert.ToByte(value);
                                int value2 = nRed + System.Convert.ToInt32(p[pos + 2]) - zRed;
                                if (value2 < 0)
                                    value2 = 0;
                                if (value2 > 255)
                                    value2 = 255;
                                p[pos + 2] = System.Convert.ToByte(value2);
                                int value3 = nGreen + System.Convert.ToInt32(p[pos + 1]) - zGreen;
                                if (value3 < 0)
                                    value3 = 0;
                                if (value3 > 255)
                                    value3 = 255;
                                p[pos + 1] = System.Convert.ToByte(value3);
                                int value4 = nBlue + System.Convert.ToInt32(p[pos]) - zBlue;
                                if (value4 < 0)
                                    value4 = 0;
                                if (value4 > 255)
                                    value4 = 255;
                                p[pos] = System.Convert.ToByte(value4);
                            }

                            pos += 4;
                        }
                    });

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
        }

        internal unsafe void SetWhite(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] > 0)
                        p[0] = p[1] = p[2] = 255;
                    else
                        p[0] = p[1] = p[2] = 0;

                    p[3] = 255;

                    p += 4;
                }
            });

            bmp.UnlockBits(bmD);
        }

        internal unsafe Bitmap? ExtractWhite(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Bitmap bRes = new Bitmap(w, h);

            BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmR = bRes.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* pR = (byte*)bmD.Scan0;
                pR += y * stride;
                byte* p = (byte*)bmR.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pR[0] == 255)
                        p[0] = p[1] = p[2] = p[3] = 255;

                    p += 4;
                    pR += 4;
                }
            });

            bmp.UnlockBits(bmD);
            bRes.UnlockBits(bmR);

            return bRes;
        }
    }
}
