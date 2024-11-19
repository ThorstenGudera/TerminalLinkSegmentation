using ConvolutionLib;
using SegmentsListLib;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvoidAGrabCutEasy
{
    public class LuminancMapOp
    {
        public float[,]? IGGLuminanceMap { get; internal set; }

        public LuminancMapOp() { }

        public async Task<float[,]?> ComputeInvLuminanceMap(Bitmap bmp)
        {
            float[,]? result = await Task.Run(() =>
            {
                float[,]? result = new float[bmp.Width, bmp.Height];
                using Bitmap b = new Bitmap(bmp);

                //1 colors
                byte[] rgb = new byte[256];
                List<Point> p = new();
                p.Add(new Point(0, 0));
                p.Add(new Point(62, 148));
                p.Add(new Point(255, 255));

                CurveSegment cuSgmt = new();
                List<BezierSegment> bz = cuSgmt.CalcBezierSegments(p.ToArray(), 0.5f);
                List<PointF> pts = cuSgmt.GetAllPoints(bz, 256, 0, 255);
                cuSgmt.MapPoints(pts, rgb);

                ColorCurves.fipbmp.GradColors(b, rgb, rgb, rgb);

                //2 blur
                int krnl = 127;
                int maxVal = 101;

                Convolution conv = new();
                conv.ProgressPlus += Conv_ProgressPlus;
                conv.CancelLoops = false;

                InvGaussGradOp igg = new InvGaussGradOp();
                igg.BGW = null;

                igg.FastZGaussian_Blur_NxN_SigmaAsDistance(bmp, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);
                conv.ProgressPlus -= Conv_ProgressPlus;

                //3 igg
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

                Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (r.Width > 0 && r.Height > 0)
                    {
                        Bitmap? iG = igg.Inv_InvGaussGrad(bmp, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                            sigma, steepness, radius, stretchValues, threshold);

                        if(iG != null)
                        {
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

                                    for(int x = 0; x < w; x++)
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
                }

                return result;
            });  
            
            return result;
        }

        public float[,]? ComputeInvLuminanceMapSync(Bitmap bmp)
        {
            float[,]? result = new float[bmp.Width, bmp.Height];
            {
                using Bitmap b = new Bitmap(bmp);

                //1 colors
                byte[] rgb = new byte[256];
                List<Point> p = new();
                p.Add(new Point(0, 0));
                p.Add(new Point(62, 148));
                p.Add(new Point(255, 255));

                CurveSegment cuSgmt = new();
                List<BezierSegment> bz = cuSgmt.CalcBezierSegments(p.ToArray(), 0.5f);
                List<PointF> pts = cuSgmt.GetAllPoints(bz, 256, 0, 255);
                cuSgmt.MapPoints(pts, rgb);

                ColorCurves.fipbmp.GradColors(b, rgb, rgb, rgb);

                //2 blur
                int krnl = 127;
                int maxVal = 101;

                Convolution conv = new();
                conv.ProgressPlus += Conv_ProgressPlus;
                conv.CancelLoops = false;

                InvGaussGradOp igg = new InvGaussGradOp();
                igg.BGW = null;

                igg.FastZGaussian_Blur_NxN_SigmaAsDistance(bmp, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);
                conv.ProgressPlus -= Conv_ProgressPlus;

                //3 igg
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

                Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (r.Width > 0 && r.Height > 0)
                    {
                        Bitmap? iG = igg.Inv_InvGaussGrad(bmp, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                            sigma, steepness, radius, stretchValues, threshold);

                        if (iG != null)
                        {
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
                    }
                }
            }

            return result;
        }

        private void Conv_ProgressPlus(object sender, ProgressEventArgs e)
        {
            
        }
    }
}
