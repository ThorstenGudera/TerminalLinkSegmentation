using ConvolutionLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AvoidAGrabCutEasy
{
    public class InvGaussGradOp
    {
        public BackgroundWorker? BGW { get; set; }
        public Bitmap? Inv_InvGaussGrad(Bitmap bmp, double alpha, GradientMode gradientMode, double divisor, int kernelLength,
            double cornerWeight, int sigma, double steepness, int radius, bool stretchValues, int threshold)
        {
            //apply Gaussian
            Convolution? conv = new Convolution();
            conv.CancelLoops = false;

            conv.ProgressPlus += Conv_ProgressPlus;

            FastZGaussian_Blur_NxN_SigmaAsDistance(bmp,
                kernelLength, cornerWeight,
                sigma, false, false,
                conv, false, steepness, radius);

            //Gradients
            using (Bitmap bmpH = new Bitmap(bmp), bmpV = new Bitmap(bmp))
            {
                EdgeDetection_Scharr_Horz(bmpH, divisor, 1, false, 127, conv, this.BGW);
                EdgeDetection_Scharr_Vert(bmpV, divisor, 1, false, 127, conv, this.BGW);

                //Magnitude
                MergeBitmaps(bmp, bmpH, bmpV, 0, 127);
                //ReplaceColors(bmp, 255, 0, 0, 0, 2, 255, 0, 0, 0);
            }

            conv.ProgressPlus -= Conv_ProgressPlus;
            conv = null;

            Bitmap? bOut = GetInvertedInvertedPic(bmp, alpha);

            if (stretchValues && bOut != null)
            {
                int[] max = GetMaxColorChannelVals(bOut);
                StretchColorChannels(bOut, max, threshold);
            }

            return bOut;
        }

        private bool EdgeDetection_Scharr_Horz(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker? bgw)
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

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1);

            return conv.Convolve_par(b, Kernel, AddVals, Bias, 255, false, true, pe, bgw, divisor);
        }

        private bool EdgeDetection_Scharr_Vert(Bitmap b, double divisor, int nWeight, bool doTransparency, int Bias, Convolution conv, System.ComponentModel.BackgroundWorker? bgw)
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

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1);

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

        private unsafe Bitmap? GetInvertedInvertedPic(Bitmap bmp, double alpha)
        {
            BitmapData? bmData = null;
            BitmapData? bmSrc = null;
            Bitmap? bmpOut = null;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 10L))
            {
                bmpOut = new Bitmap(bmp.Width, bmp.Height);
                bmData = bmpOut.LockBits(new Rectangle(0, 0, bmpOut.Width, bmpOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                bmSrc = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;
                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                byte* p = (byte*)bmData.Scan0;
                byte* pSrc = (byte*)bmSrc.Scan0;

                int pos = 0;
                for (int y = 0; y < nHeight; y++)
                {
                    pos = y * stride;
                    for (int x = 0; x < nWidth; x++)
                    {
                        //double val = 1.0 / (Math.Sqrt(1.0 + (alpha * (double)pSrc[pos] / 255))); //alpha not multiplied by 255
                        //p[pos] = p[pos + 1] = p[pos + 2] = (byte)Math.Max(Math.Min(val * 255, 255), 0);

                        double val = 255.0 / (Math.Sqrt(255.0 + (alpha * (double)pSrc[pos])) / 255);
                        p[pos] = (byte)(255 - Math.Max(Math.Min(val, 255), 0));

                        val = 255.0 / (Math.Sqrt(255.0 + (alpha * (double)pSrc[pos + 1])) / 255);
                        p[pos + 1] = (byte)(255 - Math.Max(Math.Min(val, 255), 0));

                        val = 255.0 / (Math.Sqrt(255.0 + (alpha * (double)pSrc[pos + 2])) / 255);
                        p[pos + 2] = (byte)(255 - Math.Max(Math.Min(val, 255), 0));

                        p[pos + 3] = pSrc[pos + 3];

                        pos += 4;
                    }
                }

                bmpOut.UnlockBits(bmData);
                bmp.UnlockBits(bmSrc);
            }

            return bmpOut;
        }

        private unsafe void StretchColorChannels(Bitmap bmp, int[] max, int threshold)
        {
            BitmapData? bmData = null;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 10L))
            {
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;
                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                byte* p = (byte*)bmData.Scan0;

                int addB = 255 - max[0];
                int addG = 255 - max[1];
                int addR = 255 - max[2];

                int pos = 0;
                for (int y = 0; y < nHeight; y++)
                {
                    pos = y * stride;
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (p[pos] >= threshold)
                            p[pos] = (byte)Math.Max(Math.Min(p[pos] + addB, 255), 0);

                        if (p[pos + 1] >= threshold)
                            p[pos + 1] = (byte)Math.Max(Math.Min(p[pos + 1] + addG, 255), 0);

                        if (p[pos + 2] >= threshold)
                            p[pos + 2] = (byte)Math.Max(Math.Min(p[pos + 2] + addR, 255), 0);

                        pos += 4;
                    }
                }

                bmp.UnlockBits(bmData);
            }
        }

        private unsafe int[] GetMaxColorChannelVals(Bitmap bmp)
        {
            BitmapData? bmData = null;

            int[] vals = new int[3];
            vals[0] = 255; vals[1] = 255; vals[2] = 255;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 10L))
            {
                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;
                int nWidth = bmp.Width;
                int nHeight = bmp.Height;
                bool stopL = false;

                byte* p = (byte*)bmData.Scan0;

                int maxB = 0, maxG = 0, maxR = 0;

                int pos = 0;
                for (int y = 0; y < nHeight; y++)
                {
                    pos = y * stride;
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (p[pos] > maxB)
                            maxB = p[pos];

                        if (p[pos + 1] > maxG)
                            maxG = p[pos + 1];

                        if (p[pos + 2] > maxR)
                            maxR = p[pos + 2];

                        if (maxB == 255 && maxG == 255 && maxR == 255)
                        {
                            stopL = true;
                            break;
                        }

                        pos += 4;
                    }

                    if (stopL)
                        break;
                }

                bmp.UnlockBits(bmData);

                vals[0] = maxB; vals[1] = maxG; vals[2] = maxR;
            }

            return vals;
        }

        private void Conv_ProgressPlus(object sender, ProgressEventArgs e)
        {
            this.BGW?.ReportProgress(Math.Min((int)e.CurrentProgress, 100));
        }

        public bool FastZGaussian_Blur_NxN_SigmaAsDistance(Bitmap b, int Length, double Weight, int Sigma, bool doTransparency,
            bool SrcOnSigma, Convolution conv, bool logarithmic, double steepness2,
            int Radius2)
        {
            int wh = Math.Min(b.Width, b.Height) - 1;
            if (Length > wh)
            {
                Length = wh;
                if ((Length & 0x1) != 1)
                    Length -= 1;
                Console.WriteLine("new length is: " + wh.ToString());
            }
            if ((Length & 0x1) != 1)
                return false;

            double[] KernelVector = new double[Length];

            int Radius = Length / 2;

            double a = -2.0 * Radius * Radius / Math.Log(Weight);
            double Sum = 0.0;

            for (int x = 0; x < KernelVector.Length; x++)
            {
                double dist = Math.Abs(x - Radius);
                KernelVector[x] = Math.Exp(-dist * dist / a);
                Sum += KernelVector[x];
            }

            for (int x = 0; x < KernelVector.Length; x++)
                KernelVector[x] /= Sum;

            double[] AddValVector = conv.CalculateStandardAddVals(KernelVector, Math.Min(255, b.Width - 1));

            double[] DistanceWeightsF = new double[System.Convert.ToInt32(255 * Math.Sqrt(3)) * 2 - 1 + 1];

            // Dim Radius2 As Integer = DistanceWeightsF.Length \ 2
            double a2 = -2.0 * Radius2 * Radius2 / Math.Log(steepness2);
            double Sum2 = 0.0;

            for (int x = 0; x < DistanceWeightsF.Length; x++)
            {
                double dist = Math.Abs(x - DistanceWeightsF.Length / 2);
                DistanceWeightsF[x] = Math.Exp(-dist * dist / a2);
                if (x >= DistanceWeightsF.Length / 2)
                    Sum2 += DistanceWeightsF[x];
            }

            double[] DistanceWeights = new double[DistanceWeightsF.Length / 2 + 1];

            // Dim s2 As Double = 0
            for (int x = DistanceWeightsF.Length / 2; x < DistanceWeightsF.Length; x++)
                DistanceWeights[x - DistanceWeightsF.Length / 2] = DistanceWeightsF[x] / Sum2;
            // MsgBox(s2) 'should be 1
            ProgressEventArgs pe = new ProgressEventArgs(b.Height + b.Width, 20, 1);

            try
            {
                conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, DistanceWeights, 0, Sigma, doTransparency, Math.Min(255, b.Width - 1), SrcOnSigma, pe, this.BGW, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate270FlipNone);

                conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, DistanceWeights, 0, Sigma, doTransparency, Math.Min(255, b.Width - 1), SrcOnSigma, pe, this.BGW, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }

            return true;
        }

        public bool FastZGaussian_Blur_NxN_SigmaAsDistance(Bitmap b, int Length, double Weight, int Sigma, bool doTransparency, bool SrcOnSigma, bool logarithmic)
        {
            if ((Length & 0x1) != 1)
                return false;

            double[] KernelVector = new double[Length];

            int Radius = Length / 2;

            double a = -2.0 * Radius * Radius / Math.Log(Weight);
            double Sum = 0.0;

            for (int x = 0; x < KernelVector.Length; x++)
            {
                double dist = Math.Abs(x - Radius);
                KernelVector[x] = Math.Exp(-dist * dist / a);
                Sum += KernelVector[x];
            }

            for (int x = 0; x < KernelVector.Length; x++)
                KernelVector[x] /= Sum;

            Convolution? conv = new Convolution();
            conv.CancelLoops = false;

            conv.ProgressPlus += Conv_ProgressPlus;

            double[] AddValVector = conv.CalculateStandardAddVals(KernelVector, Math.Min(255, b.Width - 1));

            ProgressEventArgs pe = new ProgressEventArgs(b.Height + b.Width, 20, 1);

            try
            {
                conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, 0, Sigma, doTransparency, Math.Min(255, b.Width - 1), SrcOnSigma, pe, this.BGW, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate270FlipNone);

                conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, 0, Sigma, doTransparency, Math.Min(255, b.Width - 1), SrcOnSigma, pe, this.BGW, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }

            conv.ProgressPlus -= Conv_ProgressPlus;

            return true;
        }

        public bool FastZGaussian_Blur_NxN_SigmaAsDistance(Bitmap b, int Length, double Weight, int Sigma, bool doTransparency, bool SrcOnSigma, bool logarithmic, ConvolutionLib.Convolution conv)
        {
            if ((Length & 0x1) != 1)
                return false;

            double[] KernelVector = new double[Length];

            int Radius = Length / 2;

            double a = -2.0 * Radius * Radius / Math.Log(Weight);
            double Sum = 0.0;

            for (int x = 0; x < KernelVector.Length; x++)
            {
                double dist = Math.Abs(x - Radius);
                KernelVector[x] = Math.Exp(-dist * dist / a);
                Sum += KernelVector[x];
            }

            for (int x = 0; x < KernelVector.Length; x++)
                KernelVector[x] /= Sum;

            double[] AddValVector = conv.CalculateStandardAddVals(KernelVector, Math.Min(255, b.Width - 1));

            ProgressEventArgs pe = new ProgressEventArgs(b.Height + b.Width, 20, 1);

            try
            {
                conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, 0, Sigma, doTransparency, Math.Min(255, b.Width - 1), SrcOnSigma, pe, this.BGW, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate270FlipNone);

                conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, 0, Sigma, doTransparency, Math.Min(255, b.Width - 1), SrcOnSigma, pe, this.BGW, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }

            return true;
        }

    }
}
