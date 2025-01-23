using ConvolutionLib;
using FloatPointPxBitmap;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoShadow
{
    internal class Fipbmp
    {
        public static bool FastZGaussian_Blur_NxN(Bitmap b, int Length, double Weight, int Sigma, bool doTransparency, 
            bool DoR, bool DoG, bool DoB, bool LeaveNotSelectedChannelsAtCurrentValue, bool SrcOnSigma, Convolution conv, bool doBoth, bool doHorz)
        {
            if ((Length & 0x1) != 1)
                return false;

            double[] KernelVector = new double[Length - 1 + 1];

            int Radius = Length / 2;

            double a = -2.0 * Radius * Radius / Math.Log(Weight);
            double Sum = 0.0;

            for (int x = 0; x <= KernelVector.Length - 1; x++)
            {
                double dist = Math.Abs(x - Radius);
                KernelVector[x] = Math.Exp(-dist * dist / a);
                Sum += KernelVector[x];
            }

            for (int x = 0; x <= KernelVector.Length - 1; x++)
                KernelVector[x] /= Sum;

            double[] AddValVector = conv.CalculateStandardAddVals(KernelVector, Math.Min(255, b.Width - 1));

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height * b.Width * 2, 0);

            if (conv.CancelLoops == false)
            {
                if (doBoth || doHorz)
                    conv.ConvolveH_Par(b, KernelVector, AddValVector, 0, Sigma, doTransparency, DoR, DoG, DoB, 
                        LeaveNotSelectedChannelsAtCurrentValue, Math.Min(255, b.Width - 1), SrcOnSigma, pe, 0);
            }

            if (conv.CancelLoops == false)
                b.RotateFlip(RotateFlipType.Rotate270FlipNone);

            if (conv.CancelLoops == false)
            {
                if (doBoth || !doHorz)
                    conv.ConvolveH_Par(b, KernelVector, AddValVector, 0, Sigma, doTransparency, DoR, DoG, DoB, 
                        LeaveNotSelectedChannelsAtCurrentValue, Math.Min(255, b.Width - 1), SrcOnSigma, pe, System.Convert.ToInt32(pe.CurrentProgress));
            }

            if (conv.CancelLoops == false)
                b.RotateFlip(RotateFlipType.Rotate90FlipNone);

            return true;
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
    }
}
