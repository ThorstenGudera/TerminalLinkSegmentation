using AvoidAGrabCutEasy.ProcOutline;
using ChainCodeFinder;
using ConvolutionLib;
using SegmentsListLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public class LuminanceMapOp
    {
        public event EventHandler<ConvolutionLib.ProgressEventArgs>? ProgressPlus;
        public float[,]? IGGLuminanceMap { get; internal set; }
        public bool Running { get; internal set; }
        public int Tolerance { get; internal set; } = 101;
        public bool ShowBitmap { get; internal set; }
        public bool ProcInnerOutlines { get; private set; } = true;

        public LuminanceMapOp() { }

        public async Task<float[,]?> ComputeInvLuminanceMap(Bitmap bmp)
        {
            float[,]? result = await Task.Run(() =>
            {
                if (!this.Running)
                {
                    this.Running = true;
                    float[,]? result = new float[bmp.Width, bmp.Height];
                    using Bitmap b = new Bitmap(bmp);

                    //1 blur
                    int krnl = 127;
                    int maxVal = 101;

                    Convolution conv = new();
                    conv.ProgressPlus += Conv_ProgressPlus;
                    conv.CancelLoops = false;

                    InvGaussGradOp igg = new InvGaussGradOp();
                    igg.BGW = null;

                    igg.FastZGaussian_Blur_NxN_SigmaAsDistance(b, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);

                    if (!this.Running)
                    {
                        if (b != null)
                            b.Dispose();
                        return null;
                    }

                    //2 igg
                    int kernelLength = 27;
                    double cornerWeight = 0.01;
                    int sigma = 255;
                    double steepness = 1E-12;
                    int radius = 340;
                    double alpha = (double)101 * 255.0;
                    GradientMode gradientMode = GradientMode.Scharr16;
                    double divisor = 8.0;
                    //bool grayscale = false;
                    bool stretchValues = true;
                    int threshold = 127;
                    bool invGaussGrad = true;
                    int pBKrnl = invGaussGrad ? 7 : 15;
                    int kernelLengthMorph = 15;

                    Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                    if (r.Width > 0 && r.Height > 0)
                    {
                        Bitmap? iG = null;

                        if (invGaussGrad)
                        {
                            iG = igg.Inv_InvGaussGrad(b, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                                    sigma, steepness, radius, stretchValues, threshold);
                        }
                        else
                        {
                            Grayscale(b);

                            using Bitmap bCopy1 = new Bitmap(b);
                            using Bitmap bCopy2 = new Bitmap(b);

                            MorphologicalProcessing2.IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.Dilate();
                            alg.BGW = null;
                            alg.SetupEx(kernelLengthMorph, kernelLengthMorph);
                            alg.ApplyGrayscale(bCopy1);
                            alg.Dispose();

                            alg = new MorphologicalProcessing2.Algorithms.Erode();
                            alg.BGW = null;
                            alg.SetupEx(kernelLengthMorph, kernelLengthMorph);
                            alg.ApplyGrayscale(bCopy2);
                            alg.Dispose();

                            Bitmap bOut = new Bitmap(bCopy1.Width, bCopy1.Height);
                            Subtract(bCopy1, bCopy2, bOut);

                            iG = bOut;
                        }

                        //now get the components and fill the inner parts of the components white,
                        //so these pixels will not be removed from the result.
                        using Bitmap iGC = new Bitmap(iG ?? throw new ArgumentNullException("iG is null"));
                        Grayscale(iGC);
                        Fipbmp fip = new Fipbmp();
                        fip.ReplaceColors(iGC, 0, 0, 0, 0, Tolerance, 255, 0, 0, 0);

                        List<ChainCode>? c = GetBoundary(iGC, 0, false);
                        if (c != null)
                        {
                            c = c.OrderByDescending(x => x.Coord.Count).ToList();

                            foreach (ChainCode cc in c)
                            {
                                if (!ChainFinder.IsInnerOutline(cc))
                                {
                                    using Graphics gx = Graphics.FromImage(iG);
                                    using GraphicsPath gP = new GraphicsPath();
                                    gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                    gP.FillMode = FillMode.Winding;
                                    gx.FillPath(Brushes.White, gP);
                                }
                                else
                                {
                                    if (this.ProcInnerOutlines)
                                    {
                                        using Graphics gx = Graphics.FromImage(iG);
                                        using (GraphicsPath gP = new GraphicsPath())
                                        {
                                            try
                                            {
                                                gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                                gx.CompositingMode = CompositingMode.SourceCopy;
                                                gx.FillPath(Brushes.Transparent, gP);
                                            }
                                            catch (Exception exc)
                                            {
                                                Console.WriteLine(exc.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (this.ShowBitmap)
                        {
                            Form fff = new Form();
                            fff.BackgroundImage = iG;
                            fff.BackgroundImageLayout = ImageLayout.Zoom;
                            fff.ShowDialog();
                            fff.BringToFront();
                        }

                        if (!this.Running)
                        {
                            if (b != null)
                                b.Dispose();
                            if (iG != null)
                                iG.Dispose();
                            return null;
                        }

                        if (iG != null && AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L))
                        {
                            //3 PostBlur
                            try
                            {
                                igg.FastZGaussian_Blur_NxN_SigmaAsDistance(iG, pBKrnl, 0.01,
                                                255, true, true, false, conv);

                                if (!this.Running)
                                    return null;

                                unsafe
                                {
                                    int w = bmp.Width;
                                    int h = bmp.Height;
                                    BitmapData bmD = iG.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                    int stride = bmD.Stride;

                                    Parallel.For(0, h, (y, loopState) =>
                                    {
                                        byte* p = (byte*)bmD.Scan0;
                                        p += y * stride;

                                        if (!this.Running)
                                            loopState.Break();

                                        for (int x = 0; x < w; x++)
                                        {
                                            Color c = Color.FromArgb(255, p[2], p[1], p[0]);
                                            result[x, y] = 1.0f - c.GetBrightness();
                                            p += 4;
                                        }
                                    });

                                    iG.UnlockBits(bmD);
                                }
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(exc.ToString());
                            }
                        }
                        else
                            MessageBox.Show("Not enough Memory.");

                        if (iG != null)
                            iG.Dispose();
                        iG = null;
                    }

                    conv.ProgressPlus -= Conv_ProgressPlus;

                    return result;
                }

                return null;
            });

            return result;
        }

        public float[,] ComputeInvLuminanceMapSync(Bitmap bmp)
        {
            float[,] result = new float[bmp.Width, bmp.Height];
            {
                using (Bitmap b = new Bitmap(bmp))
                {
                    //1 blur
                    int krnl = 127;
                    int maxVal = 101;

                    ConvolutionLib.Convolution conv = new ConvolutionLib.Convolution();
                    conv.ProgressPlus += Conv_ProgressPlus;
                    conv.CancelLoops = false;

                    InvGaussGradOp igg = new InvGaussGradOp();
                    igg.BGW = null;

                    igg.FastZGaussian_Blur_NxN_SigmaAsDistance(b, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);

                    //2 igg
                    int kernelLength = 27;
                    double cornerWeight = 0.01;
                    int sigma = 255;
                    double steepness = 1E-12;
                    int radius = 340;
                    double alpha = (double)101 * 255.0;
                    GradientMode gradientMode = GradientMode.Scharr16;
                    double divisor = 8.0;
                    //bool grayscale = false;
                    bool stretchValues = true;
                    int threshold = 127;
                    bool invGaussGrad = true;
                    int pBKrnl = invGaussGrad ? 7 : 15;
                    int kernelLengthMorph = 15;

                    Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (r.Width > 0 && r.Height > 0)
                        {
                            Bitmap? iG = null;

                            if (invGaussGrad)
                            {
                                iG = igg.Inv_InvGaussGrad(b, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                                        sigma, steepness, radius, stretchValues, threshold);
                            }
                            else
                            {
                                Grayscale(b);

                                using (Bitmap bCopy1 = new Bitmap(b))
                                using (Bitmap bCopy2 = new Bitmap(b))
                                {
                                    MorphologicalProcessing2.IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.Dilate();
                                    alg.BGW = null;
                                    alg.SetupEx(kernelLengthMorph, kernelLengthMorph);
                                    alg.ApplyGrayscale(bCopy1);
                                    alg.Dispose();

                                    alg = new MorphologicalProcessing2.Algorithms.Erode();
                                    alg.BGW = null;
                                    alg.SetupEx(kernelLengthMorph, kernelLengthMorph);
                                    alg.ApplyGrayscale(bCopy2);
                                    alg.Dispose();

                                    Bitmap bOut = new Bitmap(bCopy1.Width, bCopy1.Height);
                                    Subtract(bCopy1, bCopy2, bOut);

                                    iG = bOut;
                                }
                            }

                            //now get the components and fill the inner parts of the components white,
                            //so these pixels will not be removed from the result.
                            using Bitmap iGC = new Bitmap(iG ?? throw new ArgumentNullException("iG is null"));
                            Grayscale(iGC);
                            Fipbmp fip = new Fipbmp();
                            fip.ReplaceColors(iGC, 0, 0, 0, 0, Tolerance, 255, 0, 0, 0);

                            List<ChainCode>? c = GetBoundary(iGC, 0, false);
                            if (c != null)
                            {
                                c = c.OrderByDescending(x => x.Coord.Count).ToList();

                                foreach (ChainCode cc in c)
                                {
                                    if (!ChainFinder.IsInnerOutline(cc))
                                    {
                                        using Graphics gx = Graphics.FromImage(iG);
                                        using GraphicsPath gP = new GraphicsPath();
                                        gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                        gP.FillMode = FillMode.Winding;
                                        gx.FillPath(Brushes.White, gP);
                                    }
                                    else
                                    {
                                        if (this.ProcInnerOutlines)
                                        {
                                            using Graphics gx = Graphics.FromImage(iG);
                                            using (GraphicsPath gP = new GraphicsPath())
                                            {
                                                try
                                                {
                                                    gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                                    gx.CompositingMode = CompositingMode.SourceCopy;
                                                    gx.FillPath(Brushes.Transparent, gP);
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine(exc.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (this.ShowBitmap)
                            {
                                Form fff = new Form();
                                fff.BackgroundImage = iG;
                                fff.BackgroundImageLayout = ImageLayout.Zoom;
                                fff.ShowDialog();
                                fff.BringToFront();
                            }

                            if (iG != null && AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L))
                            {
                                //3 PostBlur
                                try
                                {
                                    igg.FastZGaussian_Blur_NxN_SigmaAsDistance(iG, pBKrnl, 0.01,
                                                                        255, true, true, false, conv);

                                    unsafe
                                    {
                                        int w = bmp.Width;
                                        int h = bmp.Height;
                                        BitmapData bmD = iG.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                        int stride = bmD.Stride;

                                        Parallel.For(0, h, y =>
                                        {
                                            byte* p = (byte*)bmD.Scan0;
                                            p += y * stride;

                                            for (int x = 0; x < w; x++)
                                            {
                                                Color c = Color.FromArgb(255, p[2], p[1], p[0]);
                                                result[x, y] = 1.0f - c.GetBrightness();
                                                p += 4;
                                            }
                                        });

                                        iG.UnlockBits(bmD);
                                    }
                                }
                                catch (Exception exc)
                                {
                                    MessageBox.Show(exc.ToString());
                                }
                            }
                            else
                                MessageBox.Show("Not enough Memory.");

                            if (iG != null)
                                iG.Dispose();
                            iG = null;
                        }
                    }

                    conv.ProgressPlus -= Conv_ProgressPlus;
                }
            }
            return result;
        }

        public unsafe void Subtract(Bitmap bCopy1, Bitmap bCopy2, Bitmap bOut)
        {
            int w = bOut.Width;
            int h = bOut.Height;
            BitmapData bmD1 = bCopy1.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmD2 = bCopy2.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmOut = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD1.Stride;

            Parallel.For(0, h, y =>
            {
                byte* pC1 = (byte*)bmD1.Scan0;
                byte* pC2 = (byte*)bmD2.Scan0;
                byte* p = (byte*)bmOut.Scan0;

                pC1 += y * stride;
                pC2 += y * stride;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    byte b = (byte)Math.Max(Math.Min((int)pC1[0] - pC2[0], 255), 0);
                    byte g = (byte)Math.Max(Math.Min((int)pC1[1] - pC2[1], 255), 0);
                    byte r = (byte)Math.Max(Math.Min((int)pC1[2] - pC2[2], 255), 0);

                    p[0] = b;
                    p[1] = g;
                    p[2] = r;
                    p[3] = 255;

                    pC1 += 4;
                    pC2 += 4;
                    p += 4;
                }
            });

            bCopy1.UnlockBits(bmD1);
            bCopy2.UnlockBits(bmD2);
            bOut.UnlockBits(bmOut);
        }

        private unsafe void Grayscale(Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            int nWidth = bmp.Width;
            int nHeight = bmp.Height;

            byte* p = (byte*)bmData.Scan0;

            int pos = 0;
            for (int y = 0; y < nHeight; y++)
            {
                pos = y * stride;
                for (int x = 0; x < nWidth; x++)
                {
                    int v = (int)Math.Max(Math.Min((double)p[pos] * 0.11 + (double)p[pos + 1] * 0.59 + (double)p[pos + 2] * 0.3, 255), 0);
                    p[pos] = p[pos + 1] = p[pos + 2] = (byte)v;

                    pos += 4;
                }
            }

            bmp.UnlockBits(bmData);
        }

        private void Conv_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            ProgressPlus?.Invoke(sender, e);
        }

        public static float[,] ComputeLuminanceMapFromPicSync(Bitmap iG)
        {
            float[,] result = new float[iG.Width, iG.Height];

            Rectangle r = new Rectangle(0, 0, iG.Width, iG.Height);

            if (r.Width > 0 && r.Height > 0)
            {
                if (iG != null)
                {
                    unsafe
                    {
                        int w = iG.Width;
                        int h = iG.Height;
                        BitmapData bmD = iG.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                        int stride = bmD.Stride;

                        Parallel.For(0, h, y =>
                        {
                            byte* p = (byte*)bmD.Scan0;
                            p += y * stride;

                            for (int x = 0; x < w; x++)
                            {
                                Color c = Color.FromArgb(255, p[2], p[1], p[0]);
                                result[x, y] = 1.0f - c.GetBrightness();
                                p += 4;
                            }
                        });

                        iG.UnlockBits(bmD);
                    }
                }
            }

            return result;
        }

        private List<ChainCode>? GetBoundary(Bitmap? upperImg, int minAlpha, bool grayScale)
        {
            List<ChainCode>? l = null;
            if (upperImg != null)
            {
                Bitmap? bmpTmp = null;
                try
                {
                    if (AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L))
                        bmpTmp = new Bitmap(upperImg);
                    else
                        throw new Exception("Not enough memory.");
                    int nWidth = bmpTmp.Width;
                    int nHeight = bmpTmp.Height;
                    ChainFinder cf = new ChainFinder();
                    l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, grayScale, 0, false, 0, false);
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
                }
            }
            return l;
        }
    }
}
