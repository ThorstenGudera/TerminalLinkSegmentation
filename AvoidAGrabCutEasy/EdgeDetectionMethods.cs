using ConvolutionLib;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace AvoidAGrabCutEasy
{
    public class EdgeDetectionMethods
    {
        private void Conv_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            e.BGW.ReportProgress(20 + (int)((e.CurrentProgress - 20.0) / e.ImgWidthHeight * 80.0));
        }

        private bool EdgeDetection_Sobel_Horz(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker bgw)
        {
            double[,] Kernel = new double[3, 3];

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] = 0.0;
            }

            Kernel[0, 0] = -1.0;
            Kernel[0, 1] = -2.0;
            Kernel[0, 2] = -1.0;
            Kernel[2, 0] = 1.0;
            Kernel[2, 1] = 2.0;
            Kernel[2, 2] = 1.0;

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1, bgw);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        private bool EdgeDetection_Sobel_Vert(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker bgw)
        {
            double[,] Kernel = new double[3, 3];

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] = 0.0;
            }

            Kernel[0, 0] = -1.0;
            Kernel[1, 0] = -2.0;
            Kernel[2, 0] = -1.0;
            Kernel[0, 2] = 1.0;
            Kernel[1, 2] = 2.0;
            Kernel[2, 2] = 1.0;

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1, bgw);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        private bool EdgeDetection_Scharr_Horz(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker bgw)
        {
            double[,] Kernel = new double[3, 3];

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] = 0.0;
            }

            Kernel[0, 0] = -3.0;
            Kernel[0, 1] = -10.0;
            Kernel[0, 2] = -3.0;
            Kernel[2, 0] = 3.0;
            Kernel[2, 1] = 10.0;
            Kernel[2, 2] = 3.0;

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1, bgw);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        private bool EdgeDetection_Scharr_Vert(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker bgw)
        {
            double[,] Kernel = new double[3, 3];

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] = 0.0;
            }

            Kernel[0, 0] = -3.0;
            Kernel[1, 0] = -10.0;
            Kernel[2, 0] = -3.0;
            Kernel[0, 2] = 3.0;
            Kernel[1, 2] = 10.0;
            Kernel[2, 2] = 3.0;

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1, bgw);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        private bool EdgeDetection_Scharr_Horz2(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker bgw)
        {
            double[,] Kernel = new double[3, 3];

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] = 0.0;
            }

            Kernel[0, 0] = -47.0;
            Kernel[0, 1] = -162.0;
            Kernel[0, 2] = -47.0;
            Kernel[2, 0] = 47.0;
            Kernel[2, 1] = 162.0;
            Kernel[2, 2] = 47.0;

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1, bgw);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        private bool EdgeDetection_Scharr_Vert2(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker bgw)
        {
            double[,] Kernel = new double[3, 3];

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] = 0.0;
            }

            Kernel[0, 0] = -47.0;
            Kernel[1, 0] = -162.0;
            Kernel[2, 0] = -47.0;
            Kernel[0, 2] = 47.0;
            Kernel[1, 2] = 162.0;
            Kernel[2, 2] = 47.0;

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1, bgw);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        public unsafe bool MergeBitmaps(Bitmap b, Bitmap f, Bitmap z, byte nThreshold, int bias)
        {
            if (AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 4L + f.Width * f.Height * 4L + z.Width * z.Height * 4L))
            {
                BitmapData? bmData = null;
                BitmapData? bmData2 = null;
                BitmapData? bmDataf = null;

                try
                {
                    bmData = f.LockBits(new Rectangle(0, 0, f.Width, f.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    bmData2 = z.LockBits(new Rectangle(0, 0, z.Width, z.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    bmDataf = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;
                    System.IntPtr Scan02 = bmData2.Scan0;
                    System.IntPtr Scanf = bmDataf.Scan0;

                    int nWidth = b.Width;
                    int nHeight = b.Height;

                    byte* p = (byte*)bmData.Scan0;
                    byte* pf = (byte*)bmData2.Scan0;
                    byte* pff = (byte*)bmDataf.Scan0;

                    for (int y = 0; y <= nHeight - 1; y++)
                    {
                        int pos = 0;
                        int posf = 0;
                        int posff = 0;

                        int nPixel = 0;

                        for (int x = 0; x <= nWidth - 1; x++)
                        {
                            nPixel = System.Convert.ToInt32(Math.Sqrt((System.Convert.ToInt32(p[pos + y * stride + x * 4]) - bias) * (System.Convert.ToInt32(p[pos + y * stride + x * 4]) - bias) + (System.Convert.ToInt32(pf[posf + y * stride + x * 4]) - bias) * (System.Convert.ToInt32(pf[posf + y * stride + x * 4]) - bias)));

                            if (nPixel < nThreshold)
                                nPixel = nThreshold;
                            if (nPixel < 0)
                                nPixel = 0;
                            if (nPixel > 255)
                                nPixel = 255;

                            pff[posff + y * stride + x * 4] = System.Convert.ToByte(nPixel);

                            nPixel = System.Convert.ToInt32(Math.Sqrt((System.Convert.ToInt32(p[pos + y * stride + x * 4 + 1]) - bias) * (System.Convert.ToInt32(p[pos + y * stride + x * 4 + 1]) - bias) + (System.Convert.ToInt32(pf[posf + y * stride + x * 4 + 1]) - bias) * (System.Convert.ToInt32(pf[posf + y * stride + x * 4 + 1]) - bias)));

                            if (nPixel < nThreshold)
                                nPixel = nThreshold;
                            if (nPixel < 0)
                                nPixel = 0;
                            if (nPixel > 255)
                                nPixel = 255;

                            pff[posff + y * stride + x * 4 + 1] = System.Convert.ToByte(nPixel);

                            nPixel = System.Convert.ToInt32(Math.Sqrt((System.Convert.ToInt32(p[pos + y * stride + x * 4 + 2]) - bias) * (System.Convert.ToInt32(p[pos + y * stride + x * 4 + 2]) - bias) + (System.Convert.ToInt32(pf[posf + y * stride + x * 4 + 2]) - bias) * (System.Convert.ToInt32(pf[posf + y * stride + x * 4 + 2]) - bias)));

                            if (nPixel < nThreshold)
                                nPixel = nThreshold;
                            if (nPixel < 0)
                                nPixel = 0;
                            if (nPixel > 255)
                                nPixel = 255;

                            pff[posff + y * stride + x * 4 + 2] = System.Convert.ToByte(nPixel);
                        }
                    }

                    f.UnlockBits(bmData);
                    z.UnlockBits(bmData2);
                    b.UnlockBits(bmDataf);

                    p = null;
                    pf = null;
                    pff = null;

                    return true;
                }
                catch
                {
                    try
                    {
                        if (bmDataf != null)
                            b.UnlockBits(bmDataf);
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (bmData != null)
                            f.UnlockBits(bmData);
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (bmData2 != null)
                            z.UnlockBits(bmData2);
                    }
                    catch
                    {
                    }
                }
            }
            return false;
        }

        public static unsafe void ReplaceColors(Bitmap bmp, int nAlpha, int nRed, int nGreen, int nBlue, int tolerance, int zAlpha, int zRed, int zGreen, int zBlue)
        {
            BitmapData? bmData = null;
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
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

        internal void GetGradMag(Bitmap bmp, GradientMode gradientMode, double divisor, BackgroundWorker bgw)
        {
            Convolution? conv = new Convolution();
            conv.CancelLoops = false;

            conv.ProgressPlus += Conv_ProgressPlus;

            //Gradients
            using (Bitmap bmpH = new Bitmap(bmp), bmpV = new Bitmap(bmp))
            {
                switch (gradientMode)
                {
                    case GradientMode.Linear:
                        GetDerivativeFH(bmpH, true);
                        GetDerivativeFV(bmpV, true);
                        break;

                    case GradientMode.Sobel:
                        EdgeDetection_Sobel_Horz(bmpH, divisor, 1, false, 127, conv, bgw);
                        EdgeDetection_Sobel_Vert(bmpV, divisor, 1, false, 127, conv, bgw);
                        break;

                    case GradientMode.Scharr16:
                        EdgeDetection_Scharr_Horz(bmpH, divisor, 1, false, 127, conv, bgw);
                        EdgeDetection_Scharr_Vert(bmpV, divisor, 1, false, 127, conv, bgw);
                        break;

                    case GradientMode.Scharr256:
                        EdgeDetection_Scharr_Horz2(bmpH, divisor, 1, false, 127, conv, bgw);
                        EdgeDetection_Scharr_Vert2(bmpV, divisor, 1, false, 127, conv, bgw);
                        break;

                    default:
                        break;
                }

                //Magnitude
                MergeBitmaps(bmp, bmpH, bmpV, 0, 127);
            }

            conv.ProgressPlus -= Conv_ProgressPlus;
            conv = null;
        }

        private unsafe void GetDerivativeFH(Bitmap bmp, bool mirrorBounds)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 10L))
            {
                using (Bitmap bSrc = new Bitmap(bmp))
                {
                    BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    int stride = bmData.Stride;

                    int nWidth = bmp.Width - 2;
                    int nHeight = bmp.Height - 2;

                    byte* p = (byte*)bmData.Scan0;
                    byte* pSrc = (byte*)bmSrc.Scan0;

                    int pos = 0;
                    for (int y = 2; y < nHeight; y++)
                    {
                        pos = y * stride + 8;
                        for (int x = 2; x < nWidth; x++)
                        {
                            for (int plane = 0; plane < 3; plane++)
                            {
                                if (pSrc[pos + plane + 4 + 3] > 0 && pSrc[pos + plane - 4 + 3] > 0)
                                    p[pos + plane] = System.Convert.ToByte(((System.Convert.ToInt32(pSrc[pos + plane + 4]) - System.Convert.ToInt32(pSrc[pos + plane - 4])) / (double)2 + 127));
                                else if (pSrc[pos + plane + 4 + 3] > 0 && pSrc[pos + plane - 4 + 3] == 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                                else if (pSrc[pos + plane + 4 + 3] == 0 && pSrc[pos + plane - 4 + 3] > 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                                else if (pSrc[pos + plane + 4 + 3] == 0 && pSrc[pos + plane - 4 + 3] == 0)
                                    p[pos + plane] = System.Convert.ToByte(127);

                                if (pSrc[pos + plane + 4 + 3] > 0 && pSrc[pos + plane - 8 + 3] == 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                                else if (pSrc[pos + plane + 8 + 3] == 0 && pSrc[pos + plane - 4 + 3] > 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                            }

                            p[pos + 3] = 255;
                            pos += 4;
                        }
                    }

                    if (mirrorBounds)
                    {
                        pos = 0;

                        for (int y = 0; y < nHeight + 2; y++)
                        {
                            pos = y * stride;
                            for (int x = 0; x < 2; x++)
                            {
                                p[pos] = p[pos + 8 - x * 4];
                                p[pos + 1] = p[pos + 8 - x * 4 + 1];
                                p[pos + 2] = p[pos + 8 - x * 4 + 2];
                                pos += 4;
                            }

                            pos = y * stride + nWidth * 4;
                            for (int x = nWidth; x < nWidth + 2; x++)
                            {
                                p[pos] = p[pos - 4 - (x - nWidth) * 4];
                                p[pos + 1] = p[pos - 4 - (x - nWidth) * 4 + 1];
                                p[pos + 2] = p[pos - 4 - (x - nWidth) * 4 + 2];
                                pos += 4;
                            }
                        }

                        for (int x = 0; x < nWidth + 2; x++)
                        {
                            pos = x * 4;
                            for (int y = 0; y < 2; y++)
                            {
                                p[pos] = p[pos + stride * 2 - y * stride];
                                p[pos + 1] = p[pos + stride * 2 - y * stride + 1];
                                p[pos + 2] = p[pos + stride * 2 - y * stride + 2];
                                pos += stride;
                            }

                            pos = x * 4 + nHeight * stride;
                            for (int y = nHeight; y < nHeight + 2; y++)
                            {
                                p[pos] = p[pos - stride - (y - nHeight) * stride];
                                p[pos + 1] = p[pos - stride - (y - nHeight) * stride + 1];
                                p[pos + 2] = p[pos - stride - (y - nHeight) * stride + 2];
                                pos += stride;
                            }
                        }
                    }

                    bmp.UnlockBits(bmData);
                    bSrc.UnlockBits(bmSrc);

                    //Form fff = new Form();
                    //fff.BackgroundImage = bmp;
                    //fff.BackgroundImageLayout = ImageLayout.Zoom;
                    //fff.ShowDialog();
                }
            }
        }

        private unsafe void GetDerivativeFV(Bitmap bmp, bool mirrorBounds)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 10L))
            {
                using (Bitmap bSrc = new Bitmap(bmp))
                {
                    BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    int stride = bmData.Stride;

                    int nWidth = bmp.Width - 2;
                    int nHeight = bmp.Height - 2;

                    byte* p = (byte*)bmData.Scan0;
                    byte* pSrc = (byte*)bmSrc.Scan0;

                    int pos = 0;
                    for (int y = 2; y < nHeight; y++)
                    {
                        pos = y * stride + 8;
                        for (int x = 2; x < nWidth; x++)
                        {
                            for (int plane = 0; plane < 3; plane++)
                            {
                                if (pSrc[pos + plane + stride + 3] > 0 && pSrc[pos + plane - stride + 3] > 0)
                                    p[pos + plane] = System.Convert.ToByte(((System.Convert.ToInt32(pSrc[pos + plane + stride]) - System.Convert.ToInt32(pSrc[pos + plane - stride])) / (double)2 + 127));
                                else if (pSrc[pos + plane + stride + 3] > 0 && pSrc[pos + plane - stride + 3] == 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                                else if (pSrc[pos + plane + stride + 3] == 0 && pSrc[pos + plane - stride + 3] > 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                                else if (pSrc[pos + plane + stride + 3] == 0 && pSrc[pos + plane - stride + 3] == 0)
                                    p[pos + plane] = System.Convert.ToByte(127);

                                if (pSrc[pos + plane + stride + 3] > 0 && pSrc[pos + plane - stride * 2 + 3] == 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                                else if (pSrc[pos + plane + stride * 2 + 3] == 0 && pSrc[pos + plane - stride + 3] > 0)
                                    p[pos + plane] = System.Convert.ToByte(127);
                            }

                            p[pos + 3] = 255;
                            pos += 4;
                        }
                    }

                    if (mirrorBounds)
                    {
                        pos = 0;

                        for (int y = 0; y < nHeight + 2; y++)
                        {
                            pos = y * stride;
                            for (int x = 0; x < 2; x++)
                            {
                                p[pos] = p[pos + 8 - x * 4];
                                p[pos + 1] = p[pos + 8 - x * 4 + 1];
                                p[pos + 2] = p[pos + 8 - x * 4 + 2];
                                pos += 4;
                            }

                            pos = y * stride + nWidth * 4;
                            for (int x = nWidth; x < nWidth + 2; x++)
                            {
                                p[pos] = p[pos - 4 - (x - nWidth) * 4];
                                p[pos + 1] = p[pos - 4 - (x - nWidth) * 4 + 1];
                                p[pos + 2] = p[pos - 4 - (x - nWidth) * 4 + 2];
                                pos += 4;
                            }
                        }

                        for (int x = 0; x < nWidth + 2; x++)
                        {
                            pos = x * 4;
                            for (int y = 0; y < 2; y++)
                            {
                                p[pos] = p[pos + stride * 2 - y * stride];
                                p[pos + 1] = p[pos + stride * 2 - y * stride + 1];
                                p[pos + 2] = p[pos + stride * 2 - y * stride + 2];
                                pos += stride;
                            }

                            pos = x * 4 + nHeight * stride;
                            for (int y = nHeight; y < nHeight + 2; y++)
                            {
                                p[pos] = p[pos - stride - (y - nHeight) * stride];
                                p[pos + 1] = p[pos - stride - (y - nHeight) * stride + 1];
                                p[pos + 2] = p[pos - stride - (y - nHeight) * stride + 2];
                                pos += stride;
                            }
                        }
                    }

                    bmp.UnlockBits(bmData);
                    bSrc.UnlockBits(bmSrc);

                    //Form fff = new Form();
                    //fff.BackgroundImage = bmp;
                    //fff.BackgroundImageLayout = ImageLayout.Zoom;
                    //fff.ShowDialog();
                }
            }
        }
    }
}