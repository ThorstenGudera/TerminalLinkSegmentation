using ConvolutionLib;
using FloatPointPxBitmap;
using System;
using System.Collections.Generic;
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
    }
}
