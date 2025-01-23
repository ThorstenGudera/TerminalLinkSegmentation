using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlineOperations
{
    internal class ThresholdMethods
    {
        public bool CancelLoops { get; internal set; }

        internal static unsafe bool ThresholdAlpha(Bitmap bmp, int th)
        {
            if (bmp == null)
                return false;

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
                    if (p[3] < th)
                        p[3] = 0;
                    else
                        p[3] = 255;

                    p += 4;
                }
            });

            bmp.UnlockBits(bmD);

            return true;
        }
    }
}
