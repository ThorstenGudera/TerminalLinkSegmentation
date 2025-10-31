using ChainCodeFinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AvoidAGrabCutEasy
{
    public class FloodFillMethods
    {
        public static bool Cancel { get; set; }

        public static void floodfill(Bitmap bmp, int x, int y, double tolerance, Color OrgPixelColor, Color ZielColor,
            int MaxIterations, bool smooth, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve)
        {
            if (tolerance == 255)
                drawOneColor(bmp, ZielColor.A, ZielColor.R, ZielColor.G, ZielColor.B);
            else if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;
                BitArray pp = new BitArray(stride * bmData.Height, false);

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                Hashtable Pixellist = new Hashtable();
                List<int> List = new List<int>();
                int zaehl = 1;

                zaehl = 1;
                Pixellist.Clear();
                List.Clear();

                List.Add(stride * -1);
                List.Add(-4);
                List.Add(4);
                List.Add(stride);

                Pixellist.Add(0, x * 4 + y * stride);

                flood(bmp, x * 4 + y * stride, tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, p);

                pp[x * 4 + y * stride] = true;

                int z = 1;

                while (z < Pixellist.Count && z < MaxIterations)
                {
                    if (FloodFillMethods.Cancel)
                        break;

                    try
                    {
                        flood(bmp, System.Convert.ToInt32(Pixellist[z]), tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, p);
                        z += 1;
                    }
                    catch
                    {
                    }
                }

                bmp.UnlockBits(bmData);

                if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                {
                    try
                    {
                        using (Bitmap bmp2 = (Bitmap)bmp.Clone())
                        {
                            writeData(bmp, ZielColor, Pixellist, smooth, smooth ? bmp2 : null, compAlpha, epsilon, useCurves, useClosedCurve);
                        }
                    }
                    catch
                    {
                    }
                }

                Pixellist.Clear();
                List.Clear();
            }
        }

        public static void floodfill(Bitmap bmp, int x, int y, double tolerance, Color OrgPixelColor, Color ZielColor,
    int MaxIterations, bool smooth, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve, BitArray pp)
        {
            if (tolerance == 255)
                drawOneColor(bmp, ZielColor.A, ZielColor.R, ZielColor.G, ZielColor.B);
            else if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                Hashtable Pixellist = new Hashtable();
                List<int> List = new List<int>();
                int zaehl = 1;

                zaehl = 1;
                Pixellist.Clear();
                List.Clear();

                List.Add(stride * -1);
                List.Add(-4);
                List.Add(4);
                List.Add(stride);

                Pixellist.Add(0, x * 4 + y * stride);

                flood(bmp, x * 4 + y * stride, tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, p);

                pp[x * 4 + y * stride] = true;

                int z = 1;

                while (z < Pixellist.Count && z < MaxIterations)
                {
                    if (FloodFillMethods.Cancel)
                        break;

                    try
                    {
                        flood(bmp, System.Convert.ToInt32(Pixellist[z]), tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, p);
                        z += 1;
                    }
                    catch
                    {
                    }
                }

                bmp.UnlockBits(bmData);

                if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                {
                    try
                    {
                        using (Bitmap bmp2 = (Bitmap)bmp.Clone())
                        {
                            writeData(bmp, ZielColor, Pixellist, smooth, smooth ? bmp2 : null, compAlpha, epsilon, useCurves, useClosedCurve);
                        }
                    }
                    catch
                    {
                    }
                }

                Pixellist.Clear();
                List.Clear();
            }
        }

        public static void floodfill(Bitmap bmp, int x, int y, double tolerance, Color OrgPixelColor, Color ZielColor, int MaxIterations, double edges, bool smooth, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve)
        {
            if (tolerance == 255 && edges == 255)
                drawOneColor(bmp, ZielColor.A, ZielColor.R, ZielColor.G, ZielColor.B);
            else if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;
                BitArray pp = new BitArray(stride * bmData.Height, false);

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                Hashtable Pixellist = new Hashtable();
                List<int> List = new List<int>();
                int zaehl = 1;

                zaehl = 1;
                Pixellist.Clear();
                List.Clear();

                List.Add(stride * -1);
                List.Add(-4);
                List.Add(4);
                List.Add(stride);

                Pixellist.Add(0, x * 4 + y * stride);

                flood(bmp, x * 4 + y * stride, tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, p, edges);

                pp[x * 4 + y * stride] = true;

                int z = 1;

                while (z < Pixellist.Count && z < MaxIterations)
                {
                    if (FloodFillMethods.Cancel)
                        break;

                    try
                    {
                        flood(bmp, System.Convert.ToInt32(Pixellist[z]), tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, p, edges);
                        z += 1;
                    }
                    catch
                    {
                    }
                }

                bmp.UnlockBits(bmData);

                // writeData(bmp, ZielColor, Pixellist)

                if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                {
                    try
                    {
                        using (Bitmap bmp2 = (Bitmap)bmp.Clone())
                        {
                            writeData(bmp, ZielColor, Pixellist, smooth, smooth ? bmp2 : null, compAlpha, epsilon, useCurves, useClosedCurve);
                        }
                    }
                    catch
                    {
                    }
                }

                Pixellist.Clear();
                List.Clear();
            }
        }

        public static void flood(Bitmap bmp, int x, double tolerance, Color OrgPixelColor, BitmapData bmData, BitArray pp, Hashtable Pixellist, List<int> List, ref int zaehl, byte[] p)
        {
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            foreach (int i in List)
            {
                int z = (x + i);
                if ((z >= 0) && (z < stride * bmp.Height))
                {
                    if ((p[z + 3] == 0 && OrgPixelColor.A == 0) || ((p[z + 3] >= (OrgPixelColor.A - tolerance)) && (p[z + 3] <= (OrgPixelColor.A + tolerance)) && (p[z + 2] >= (OrgPixelColor.R - tolerance)) && (p[z + 2] <= (OrgPixelColor.R + tolerance)) && (p[z + 1] >= (OrgPixelColor.G - tolerance)) && (p[z + 1] <= (OrgPixelColor.G + tolerance)) && (p[z] >= (OrgPixelColor.B - tolerance)) && (p[z] <= (OrgPixelColor.B + tolerance))))
                    {
                        try
                        {
                            if (pp[z] == false)
                            {
                                if ((i == 4) && (x % stride != (stride - 4)))
                                {
                                    Pixellist.Add(zaehl, z);
                                    zaehl += 1;
                                    pp[z] = true;
                                }
                                if ((i == -4) && (x % stride != 0))
                                {
                                    Pixellist.Add(zaehl, z);
                                    zaehl += 1;
                                    pp[z] = true;
                                }
                                if ((i == -stride) && (x >= stride))
                                {
                                    Pixellist.Add(zaehl, z);
                                    zaehl += 1;
                                    pp[z] = true;
                                }
                                if ((i == stride) && (x < stride * (bmp.Height - 1)))
                                {
                                    Pixellist.Add(zaehl, z);
                                    zaehl += 1;
                                    pp[z] = true;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static void flood(Bitmap bmp, int x, double tolerance, Color OrgPixelColor, BitmapData bmData, BitArray pp, Hashtable Pixellist, List<int> List, ref int zaehl, byte[] p, double edges)
        {
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            foreach (int i in List)
            {
                int z = (x + i);
                if ((z >= 0) && (z < stride * bmp.Height))
                {
                    if ((p[z + 3] == 0 && OrgPixelColor.A == 0) || ((p[z + 3] >= (OrgPixelColor.A - tolerance)) && (p[z + 3] <= (OrgPixelColor.A + tolerance)) && (p[z + 2] >= (OrgPixelColor.R - tolerance)) && (p[z + 2] <= (OrgPixelColor.R + tolerance)) && (p[z + 1] >= (OrgPixelColor.G - tolerance)) && (p[z + 1] <= (OrgPixelColor.G + tolerance)) && (p[z] >= (OrgPixelColor.B - tolerance)) && (p[z] <= (OrgPixelColor.B + tolerance))))
                    {
                        if (((p[z + 3] >= (p[x + 3] - edges)) && (p[z + 3] <= (p[x + 3] + edges)) && (p[z + 2] >= (p[x + 2] - edges)) && (p[z + 2] <= (p[x + 2] + edges)) && (p[z + 1] >= (p[x + 1] - edges)) && (p[z + 1] <= (p[x + 1] + edges)) && (p[z] >= (p[x] - edges)) && (p[z] <= (p[x] + edges))))
                        {
                            try
                            {
                                if (pp[z] == false)
                                {
                                    if ((i == 4) && (x % stride != (stride - 4)))
                                    {
                                        Pixellist.Add(zaehl, z);
                                        zaehl += 1;
                                        pp[z] = true;
                                    }
                                    if ((i == -4) && (x % stride != 0))
                                    {
                                        Pixellist.Add(zaehl, z);
                                        zaehl += 1;
                                        pp[z] = true;
                                    }
                                    if ((i == -stride) && (x >= stride))
                                    {
                                        Pixellist.Add(zaehl, z);
                                        zaehl += 1;
                                        pp[z] = true;
                                    }
                                    if ((i == stride) && (x < stride * (bmp.Height - 1)))
                                    {
                                        Pixellist.Add(zaehl, z);
                                        zaehl += 1;
                                        pp[z] = true;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public static void writeData(Bitmap bmp, Color z, Hashtable Pixellist, bool smooth, Bitmap? bmpCopy, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                if (smooth)
                {
                    if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
                    {
                        Bitmap? b = null;
                        BitmapData? bmD = null;

                        try
                        {
                            b = new Bitmap(bmp.Width, bmp.Height);
                            bmD = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                            int stride2 = bmD.Stride;
                            System.IntPtr Scan02 = bmD.Scan0;

                            byte[] p2 = new byte[(bmD.Stride * bmD.Height) - 1 + 1];
                            Marshal.Copy(bmD.Scan0, p2, 0, p2.Length);

                            foreach (int i in Pixellist.Values)
                            {
                                if (FloodFillMethods.Cancel)
                                    break;
                                p2[i] = System.Convert.ToByte(z.B);
                                p2[i + 1] = System.Convert.ToByte(z.G);
                                p2[i + 2] = System.Convert.ToByte(z.R);
                                p2[i + 3] = System.Convert.ToByte(z.A);
                            }

                            Marshal.Copy(p2, 0, bmD.Scan0, p2.Length);
                            b.UnlockBits(bmD);

                            List<ChainCode> fList = new List<ChainCode>();
                            ChainFinder fbmp = new ChainFinder();
                            fbmp.AllowNullCells = false;

                            if (compAlpha)
                                fList = fbmp.GetOutline(b, 0, false, 0, false, 0, false);
                            else
                                fList = fbmp.GetOutline(b, 0, true, 0, false, 0, false);

                            fbmp.RemoveOutlines(fList, 4);

                            if (fList.Count > 0)
                            {
                                using (GraphicsPath fPath = GetPath(fList, epsilon, useCurves, useClosedCurve))
                                {
                                    if (fPath != null)
                                    {
                                        try
                                        {
                                            using (Graphics g = Graphics.FromImage(b))
                                            using (SolidBrush sb = new SolidBrush(z)
    )
                                            {
                                                g.Clear(Color.Transparent);
                                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                                g.FillPath(sb, fPath);
                                            }

                                            using (Graphics g = Graphics.FromImage(bmp))
                                            {
                                                g.DrawImage(b, 0, 0, bmp.Width, bmp.Height);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            try
                            {
                                if (b != null && bmD != null)
                                    b.UnlockBits(bmD);
                            }
                            catch
                            {
                            }
                        }
                        finally
                        {
                            if (b != null)
                                b.Dispose();
                        }
                    }
                }
                else
                {
                    BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;

                    byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                    foreach (int i in Pixellist.Values)
                    {
                        if (FloodFillMethods.Cancel)
                            break;
                        p[i] = System.Convert.ToByte(z.B);
                        p[i + 1] = System.Convert.ToByte(z.G);
                        p[i + 2] = System.Convert.ToByte(z.R);
                        p[i + 3] = System.Convert.ToByte(z.A);
                    }

                    Marshal.Copy(p, 0, bmData.Scan0, p.Length);
                    bmp.UnlockBits(bmData);
                }
            }
        }

        internal static GraphicsPath GetPath(List<ChainCode> flist, double epsilon, bool useCurves, bool useClosedCurve)
        {
            GraphicsPath fPath = new GraphicsPath();
            ChainFinder fBmp = new ChainFinder();

            for (int i = 0; i <= flist.Count - 1; i++)
            {
                using (GraphicsPath gPath = new GraphicsPath())
                {
                    ChainCode c = flist[i];
                    List<Point> lList = c.Coord;
                    lList = fBmp.ApproximateLines(lList, epsilon);
                    lList = fBmp.RemoveColinearity(lList, true);

                    if (useCurves)
                    {
                        if (useClosedCurve)
                        {
                            bool rlp = false;
                            if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                            {
                                lList.RemoveAt(lList.Count - 1);
                                rlp = true;
                            }
                            gPath.AddClosedCurve(lList.ToArray());
                            if (rlp)
                                lList.Add(lList[0]);
                        }
                        else
                            gPath.AddCurve(lList.ToArray());
                    }
                    else
                        gPath.AddLines(lList.ToArray());

                    gPath.CloseFigure();

                    if (gPath != null)
                    {
                        try
                        {
                            if (gPath.PathPoints.Length > 1)
                                fPath.AddPath(gPath, false);
                            else
                            {
                                PointF p = gPath.PathPoints[0];
                                fPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return fPath;
        }

        public static void floodfill(Bitmap bmp, int x, int y, double tolerance, Color OrgPixelColor, Color ZielColor, int MaxIterations, double addVal, double minVal, bool smooth, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve)
        {
            if (tolerance == 255)
                drawOneColor(bmp, ZielColor.A, ZielColor.R, ZielColor.G, ZielColor.B);
            else if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                tolerance = System.Convert.ToDouble(tolerance * Math.Sqrt(3.0)); // we use a color distance (p2 norm of the vector) so increase the tolerance up to 4 times 'was 2
                addVal = System.Convert.ToDouble(addVal * Math.Sqrt(3.0));

                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;
                BitArray pp = new BitArray(stride * bmData.Height, false);

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                Dictionary<int, int> Pixellist = new Dictionary<int, int>();
                Dictionary<int, double> Pixellist2 = new Dictionary<int, double>();
                List<int> List = new List<int>();
                int zaehl = 1;

                zaehl = 1;
                Pixellist.Clear();
                List.Clear();

                List.Add(stride * -1);
                List.Add(-4);
                List.Add(4);
                List.Add(stride);

                Pixellist.Add(0, x * 4 + y * stride);
                Pixellist2.Add(0, 1000);

                flood(bmp, x * 4 + y * stride, tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, Pixellist2, addVal, minVal, p);

                pp[x * 4 + y * stride] = true;

                int z = 1;

                while (z < Pixellist.Count && z < MaxIterations)
                {
                    if (FloodFillMethods.Cancel)
                        break;
                    try
                    {
                        if (Pixellist2[z] >= minVal)
                            flood(bmp, System.Convert.ToInt32(Pixellist[z]), tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, Pixellist2, addVal, minVal, p);
                        z += 1;
                    }
                    catch
                    {
                    }
                }

                bmp.UnlockBits(bmData);

                if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                {
                    try
                    {
                        using (Bitmap bmp2 = (Bitmap)bmp.Clone())
                        {
                            writeData2(bmp, Pixellist, Pixellist2, ZielColor, minVal, smooth, smooth ? bmp2 : null, compAlpha, epsilon, useCurves, useClosedCurve);
                        }
                    }
                    catch
                    {
                    }
                }

                Pixellist.Clear();
                List.Clear();
            }
        }

        public static void floodfill(Bitmap bmp, int x, int y, double tolerance, Color OrgPixelColor, Color ZielColor, int MaxIterations, double addVal, double minVal, double edges, bool smooth, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve)
        {
            if (tolerance == 255 && edges == 255)
                drawOneColor(bmp, ZielColor.A, ZielColor.R, ZielColor.G, ZielColor.B);
            else if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                tolerance = System.Convert.ToDouble(tolerance * Math.Sqrt(3.0)); // we use a color distance (p2 norm of the vector) so increase the tolerance up to 4 times 'was 2
                addVal = System.Convert.ToDouble(addVal * Math.Sqrt(3.0));
                edges = System.Convert.ToDouble(edges * Math.Sqrt(3.0));

                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;
                BitArray pp = new BitArray(stride * bmData.Height, false);

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                Dictionary<int, int> Pixellist = new Dictionary<int, int>();
                Dictionary<int, double> Pixellist2 = new Dictionary<int, double>();
                List<int> List = new List<int>();
                int zaehl = 1;

                zaehl = 1;
                Pixellist.Clear();
                List.Clear();

                List.Add(stride * -1);
                List.Add(-4);
                List.Add(4);
                List.Add(stride);

                Pixellist.Add(0, x * 4 + y * stride);
                Pixellist2.Add(0, 1000);

                flood(bmp, x * 4 + y * stride, tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, Pixellist2, addVal, minVal, p, edges);

                pp[x * 4 + y * stride] = true;

                int z = 1;

                while (z < Pixellist.Count && z < MaxIterations)
                {
                    if (FloodFillMethods.Cancel)
                        break;
                    try
                    {
                        if (Pixellist2[z] >= minVal)
                            flood(bmp, System.Convert.ToInt32(Pixellist[z]), tolerance, OrgPixelColor, bmData, pp, Pixellist, List, ref zaehl, Pixellist2, addVal, minVal, p, edges);
                        z += 1;
                    }
                    catch
                    {
                    }
                }

                bmp.UnlockBits(bmData);

                // writeData2(bmp, Pixellist, Pixellist2, ZielColor, minVal)

                if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                {
                    try
                    {
                        using (Bitmap bmp2 = (Bitmap)bmp.Clone())
                        {
                            writeData2(bmp, Pixellist, Pixellist2, ZielColor, minVal, smooth, smooth ? bmp2 : null, compAlpha, epsilon, useCurves, useClosedCurve);
                        }
                    }
                    catch
                    {
                    }
                }

                Pixellist.Clear();
                List.Clear();
            }
        }

        public static void flood(Bitmap bmp, int x, double tolerance, Color OrgPixelColor, BitmapData bmData, BitArray pp, Dictionary<int, int> Pixellist, List<int> List, ref int zaehl, Dictionary<int, double> Pixellist2, double addVal, double minVal, byte[] p)
        {
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            foreach (int i in List)
            {
                int z = (x + i);
                if ((z >= 0) && (z < stride * bmp.Height))
                {
                    double distance = Math.Sqrt((System.Convert.ToInt32(p[z + 3]) - OrgPixelColor.A) * (System.Convert.ToInt32(p[z + 3]) - OrgPixelColor.A) + (System.Convert.ToInt32(p[z + 2]) - OrgPixelColor.R) * (System.Convert.ToInt32(p[z + 2]) - OrgPixelColor.R) + (System.Convert.ToInt32(p[z + 1]) - OrgPixelColor.G) * (System.Convert.ToInt32(p[z + 1]) - OrgPixelColor.G) + (System.Convert.ToInt32(p[z]) - OrgPixelColor.B) * (System.Convert.ToInt32(p[z]) - OrgPixelColor.B));

                    if ((p[z + 3] == 0 && OrgPixelColor.A == 0) || (distance <= tolerance))
                    {
                        try
                        {
                            if (pp[z] == false)
                                SetPixellist(i, x, z, stride, pp, bmp, Pixellist, ref zaehl, Pixellist2, 1000);
                        }
                        catch
                        {
                        }
                    }
                    else if (addVal > 0 && (distance > tolerance) && (distance < tolerance + addVal))
                    {
                        double f = distance - tolerance; // a positive number in range from 0 to addVal
                                                         // number close to 1000 means close to normal floodfill bound, smaller number means bigger distance, this is kind of a distance ramp
                        double z2 = 1000.0 - (f / addVal * 1000.0); // not used at current stage except for testing against minVal; a number ("weight" for the distance that exceeds the "normal" distance) in range from 0 to 1000

                        if (z2 > minVal)
                        {
                            try
                            {
                                if (pp[z] == false)
                                    SetPixellist(i, x, z, stride, pp, bmp, Pixellist, ref zaehl, Pixellist2, z2);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public static void flood(Bitmap bmp, int x, double tolerance, Color OrgPixelColor, BitmapData bmData, BitArray pp, Dictionary<int, int> Pixellist, List<int> List, ref int zaehl, Dictionary<int, double> Pixellist2, double addVal, double minVal, byte[] p, double edges)
        {
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            foreach (int i in List)
            {
                int z = (x + i);
                if ((z >= 0) && (z < stride * bmp.Height))
                {
                    double distance = Math.Sqrt((System.Convert.ToInt32(p[z + 3]) - OrgPixelColor.A) * (System.Convert.ToInt32(p[z + 3]) - OrgPixelColor.A) + (System.Convert.ToInt32(p[z + 2]) - OrgPixelColor.R) * (System.Convert.ToInt32(p[z + 2]) - OrgPixelColor.R) + (System.Convert.ToInt32(p[z + 1]) - OrgPixelColor.G) * (System.Convert.ToInt32(p[z + 1]) - OrgPixelColor.G) + (System.Convert.ToInt32(p[z]) - OrgPixelColor.B) * (System.Convert.ToInt32(p[z]) - OrgPixelColor.B));

                    double edge = Math.Sqrt((System.Convert.ToInt32(p[z + 3]) - System.Convert.ToInt32(p[x + 3])) * (System.Convert.ToInt32(p[z + 3]) - System.Convert.ToInt32(p[x + 3])) + (System.Convert.ToInt32(p[z + 2]) - System.Convert.ToInt32(p[x + 2])) * (System.Convert.ToInt32(p[z + 2]) - System.Convert.ToInt32(p[x + 2])) + (System.Convert.ToInt32(p[z + 1]) - System.Convert.ToInt32(p[x + 1])) * (System.Convert.ToInt32(p[z + 1]) - System.Convert.ToInt32(p[x + 1])) + (System.Convert.ToInt32(p[z]) - System.Convert.ToInt32(p[x])) * (System.Convert.ToInt32(p[z]) - System.Convert.ToInt32(p[x])));

                    if ((p[z + 3] == 0 && OrgPixelColor.A == 0) || ((distance <= tolerance) && edge < edges))
                    {
                        try
                        {
                            if (pp[z] == false)
                                SetPixellist(i, x, z, stride, pp, bmp, Pixellist, ref zaehl, Pixellist2, 1000);
                        }
                        catch
                        {
                        }
                    }
                    else if (addVal > 0 && (distance > tolerance) && (distance < tolerance + addVal) && (edge < edges))
                    {
                        double f = distance - tolerance;
                        double z2 = 1000.0 - (f / addVal * 1000.0);

                        if (z2 > minVal)
                        {
                            try
                            {
                                if (pp[z] == false)
                                    SetPixellist(i, x, z, stride, pp, bmp, Pixellist, ref zaehl, Pixellist2, z2);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public static void writeData2(Bitmap bmp, Dictionary<int, int> Pixellist, Dictionary<int, double> Pixellist2, Color z,
            double minVal, bool smooth, Bitmap? bmpCopy, bool compAlpha, double epsilon, bool useCurves, bool useClosedCurve)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                if (smooth)
                {
                    if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
                    {
                        Bitmap? b = null;
                        BitmapData? bmD = null;

                        try
                        {
                            b = new Bitmap(bmp.Width, bmp.Height);
                            bmD = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                            int stride2 = bmD.Stride;
                            System.IntPtr Scan02 = bmD.Scan0;

                            byte[] p2 = new byte[(bmD.Stride * bmD.Height) - 1 + 1];
                            Marshal.Copy(bmD.Scan0, p2, 0, p2.Length);

                            foreach (KeyValuePair<int, int> f in Pixellist)
                            {
                                if (FloodFillMethods.Cancel)
                                    break;
                                double multiplier = Pixellist2[f.Key]; // unused at current stage

                                if (multiplier == 1000)
                                {
                                    p2[f.Value] = System.Convert.ToByte(z.B);
                                    p2[f.Value + 1] = System.Convert.ToByte(z.G);
                                    p2[f.Value + 2] = System.Convert.ToByte(z.R);
                                    p2[f.Value + 3] = System.Convert.ToByte(z.A);
                                }
                                else if (multiplier > minVal)
                                {
                                    // int pp3 = (int)(z.A * multiplier / 1000.0);

                                    // double p1 = (double)(p[f.Value] + p[f.Value] * ((255 - pp3) / 255.0));
                                    // double p2 = z.B * multiplier / 1000.0;
                                    // int value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                                    // p[f.Value] = (byte)value;

                                    // p1 = (double)(p[f.Value + 1] + p[f.Value + 1] * ((255 - pp3) / 255.0));
                                    // p2 = z.G * multiplier / 1000.0;
                                    // value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                                    // p[f.Value + 1] = (byte)value;

                                    // p1 = (double)(p[f.Value + 2] + p[f.Value + 2] * ((255 - pp3) / 255.0));
                                    // p2 = z.R * multiplier / 1000.0;
                                    // value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                                    // p[f.Value + 2] = (byte)value;

                                    // p1 = (double)(p[f.Value + 3] + p[f.Value + 3] * ((255 - pp3) / 255.0));
                                    // p2 = z.A * multiplier / 1000.0;
                                    // value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                                    // p[f.Value + 3] = (byte)value;   

                                    p2[f.Value] = System.Convert.ToByte(z.B);
                                    p2[f.Value + 1] = System.Convert.ToByte(z.G);
                                    p2[f.Value + 2] = System.Convert.ToByte(z.R);
                                    p2[f.Value + 3] = System.Convert.ToByte(z.A);
                                }
                            }

                            Marshal.Copy(p2, 0, bmD.Scan0, p2.Length);
                            b.UnlockBits(bmD);

                            List<ChainCode> fList = new List<ChainCode>();
                            ChainFinder fbmp = new ChainFinder();
                            fbmp.AllowNullCells = false;

                            if (compAlpha)
                                fList = fbmp.GetOutline(b, 0, false, 0, false, 0, false);
                            else
                                fList = fbmp.GetOutline(b, 0, true, 0, false, 0, false);

                            fbmp.RemoveOutlines(fList, 4);

                            if (fList.Count > 0)
                            {
                                using (GraphicsPath fPath = GetPath(fList, epsilon, useCurves, useClosedCurve))
                                {
                                    if (fPath != null)
                                    {
                                        try
                                        {
                                            using (Graphics g = Graphics.FromImage(b))
                                            using (SolidBrush sb = new SolidBrush(z)
    )
                                            {
                                                g.Clear(Color.Transparent);
                                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                                g.FillPath(sb, fPath);
                                            }

                                            using (Graphics g = Graphics.FromImage(bmp))
                                            {
                                                g.DrawImage(b, 0, 0, bmp.Width, bmp.Height);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            try
                            {
                                if (b != null && bmD != null)
                                    b.UnlockBits(bmD);
                            }
                            catch
                            {
                            }
                        }
                        finally
                        {
                            if (b != null)
                                b.Dispose();
                        }
                    }
                }
                else
                {
                    BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;

                    byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                    foreach (KeyValuePair<int, int> f in Pixellist)
                    {
                        if (FloodFillMethods.Cancel)
                            break;
                        double multiplier = Pixellist2[f.Key]; // unused at current stage

                        if (multiplier == 1000)
                        {
                            p[f.Value] = System.Convert.ToByte(z.B);
                            p[f.Value + 1] = System.Convert.ToByte(z.G);
                            p[f.Value + 2] = System.Convert.ToByte(z.R);
                            p[f.Value + 3] = System.Convert.ToByte(z.A);
                        }
                        else if (multiplier > minVal)
                        {
                            // int pp3 = (int)(z.A * multiplier / 1000.0);

                            // double p1 = (double)(p[f.Value] + p[f.Value] * ((255 - pp3) / 255.0));
                            // double p2 = z.B * multiplier / 1000.0;
                            // int value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                            // p[f.Value] = (byte)value;

                            // p1 = (double)(p[f.Value + 1] + p[f.Value + 1] * ((255 - pp3) / 255.0));
                            // p2 = z.G * multiplier / 1000.0;
                            // value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                            // p[f.Value + 1] = (byte)value;

                            // p1 = (double)(p[f.Value + 2] + p[f.Value + 2] * ((255 - pp3) / 255.0));
                            // p2 = z.R * multiplier / 1000.0;
                            // value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                            // p[f.Value + 2] = (byte)value;

                            // p1 = (double)(p[f.Value + 3] + p[f.Value + 3] * ((255 - pp3) / 255.0));
                            // p2 = z.A * multiplier / 1000.0;
                            // value = Math.Max(Math.Min((int)((p1 + p2) / 2.0), 255), 0);
                            // p[f.Value + 3] = (byte)value;   

                            p[f.Value] = System.Convert.ToByte(z.B);
                            p[f.Value + 1] = System.Convert.ToByte(z.G);
                            p[f.Value + 2] = System.Convert.ToByte(z.R);
                            p[f.Value + 3] = System.Convert.ToByte(z.A);
                        }
                    }

                    Marshal.Copy(p, 0, bmData.Scan0, p.Length);
                    bmp.UnlockBits(bmData);
                }
            }
        }

        public static void drawOneColor(Bitmap bmp, int nAlpha, int nRed, int nGreen, int nBlue)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                BitmapData? bmData = null;

                try
                {
                    bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                    int scanline = bmData.Stride;

                    int nWidth = bmp.Width;
                    int nHeight = bmp.Height;

                    for (int y = 0; y <= nHeight - 1; y++)
                    {
                        if (FloodFillMethods.Cancel)
                            break;
                        int posPixel = y * scanline;
                        for (int x = 0; x <= nWidth - 1; x++)
                        {
                            int value = nAlpha;
                            if (value < 0)
                                value = 0;
                            if (value > 255)
                                value = 255;
                            p[posPixel + 3] = System.Convert.ToByte(Math.Max(Math.Min(System.Convert.ToDouble(value) * (System.Convert.ToDouble(p[posPixel + 3]) / 255.0), 255), 0));

                            int value2 = nRed;
                            if (value2 < 0)
                                value2 = 0;
                            if (value2 > 255)
                                value2 = 255;
                            p[posPixel + 2] = System.Convert.ToByte(value2);

                            int value3 = nGreen;
                            if (value3 < 0)
                                value3 = 0;
                            if (value3 > 255)
                                value3 = 255;
                            p[posPixel + 1] = System.Convert.ToByte(value3);

                            int value4 = nBlue;
                            if (value4 < 0)
                                value4 = 0;
                            if (value4 > 255)
                                value4 = 255;
                            p[posPixel] = System.Convert.ToByte(value4);

                            posPixel += 4;
                        }
                    }

                    Marshal.Copy(p, 0, bmData.Scan0, p.Length);
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

        // Pixellist2 isnt really used at this stage, maybe remove or comment it out, but probably it will come back later...
        private static void SetPixellist(int i, int x, int z, int stride, BitArray pp, Bitmap bmp, Dictionary<int, int> Pixellist, ref int zaehl, Dictionary<int, double> Pixellist2, double factor)
        {
            if ((i == 4) && (x % stride != (stride - 4)))
            {
                Pixellist.Add(zaehl, z);
                Pixellist2.Add(zaehl, factor);
                zaehl += 1;
                pp[z] = true;
            }
            if ((i == -4) && (x % stride != 0))
            {
                Pixellist.Add(zaehl, z);
                Pixellist2.Add(zaehl, factor);
                zaehl += 1;
                pp[z] = true;
            }
            if ((i == -stride) && (x >= stride))
            {
                Pixellist.Add(zaehl, z);
                Pixellist2.Add(zaehl, factor);
                zaehl += 1;
                pp[z] = true;
            }
            if ((i == stride) && (x < stride * (bmp.Height - 1)))
            {
                Pixellist.Add(zaehl, z);
                Pixellist2.Add(zaehl, factor);
                zaehl += 1;
                pp[z] = true;
            }
        }
    }
}
