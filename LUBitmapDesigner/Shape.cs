using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LUBitmapDesigner
{
    public abstract class Shape : IDisposable
    {
        internal static int Cnt = 0;
        public int ID { get; set; }
        public RectangleF Bounds { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; } = 1.0f;
        public MergeOperation MergeOperation { get; set; } = MergeOperation.Normal;
        public bool ForceHQRendering { get; set; } = true;
        public float Opacity { get; set; } = 1.0f;
        public abstract void Draw(Graphics gx);
        public abstract void Dispose();

        public void Draw(Bitmap? background)
        {
            if (this.MergeOperation == MergeOperation.Normal && background != null)
            {
                using Graphics gx = Graphics.FromImage(background);
                this.Draw(gx);
            }
            else if (background != null)
            {
                //we change the Bounds temporilarily later
                RectangleF boudnsBackUp = this.Bounds;
                //get rects one for the unrotated pic, one for the rotated pic
                RectangleF drawRectZ1 = GetCompatibleGraphicsPath(this.Zoom, new SizeF(background.Width, background.Height));
                RectangleF drawRectZOffset = GetCompatibleGraphicsPathNoRotation(this.Zoom, new SizeF(background.Width, background.Height));

                //offset for rotation
                float rotOffsetX = drawRectZOffset.X - drawRectZ1.X;
                float rotOffsetY = drawRectZOffset.Y - drawRectZ1.Y;

                //set new bounds for tmp pic
                this.Bounds = new RectangleF(rotOffsetX / this.Zoom, rotOffsetY / this.Zoom, drawRectZOffset.Width / this.Zoom, drawRectZOffset.Height / this.Zoom);

                int bmpWidth = Math.Max((int)Math.Ceiling(drawRectZ1.Width), 1);
                int bmpHeight = Math.Max((int)Math.Ceiling(drawRectZ1.Height), 1);

                using Bitmap? bmpTmp = new(bmpWidth, bmpHeight);
                using Graphics g = Graphics.FromImage(bmpTmp);

                this.Draw(g);

                //Form fff = new Form();
                //fff.BackgroundImage = bmpTmp;
                //fff.BackgroundImageLayout = ImageLayout.Zoom;
                //fff.ShowDialog();

                //get the rect for merging
                Rectangle rcFG = new Rectangle((int)drawRectZ1.X, (int)drawRectZ1.Y,
                    (int)Math.Ceiling(drawRectZ1.Width), (int)Math.Ceiling(drawRectZ1.Height));

                Merge(bmpTmp, background, rcFG);

                this.Bounds = boudnsBackUp;
            }
        }
        private unsafe void Merge(Bitmap bmpUpper, Bitmap? background, Rectangle bounds)
        {
            if (background != null && bmpUpper != null)
            {
                Rectangle rcDraw = new Rectangle(0, 0, background.Width, background.Height);
                rcDraw.Intersect(bounds);

                BitmapData bmU = bmpUpper.LockBits(new Rectangle(0, 0, bmpUpper.Width, bmpUpper.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bmL = background.LockBits(new Rectangle(0, 0, background.Width, background.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int strideU = bmU.Stride;
                int strideL = bmL.Stride;

                int bW = background.Width;
                int bH = background.Height;
                int uW = bmpUpper.Width;
                int uH = bmpUpper.Height;

                int xx = rcDraw.X;
                int yy = rcDraw.Y;
                int w = rcDraw.Width;
                int h = rcDraw.Height;
                int h2 = yy + h;

                Parallel.For(yy, h2, y =>
                {
                    byte* pL = (byte*)bmL.Scan0;
                    byte* pU = (byte*)bmU.Scan0;

                    int yyy = y - yy + Math.Max(-bounds.Y, 0);

                    float opc = this.Opacity;

                    for (int x = xx, xxx = Math.Max(-bounds.X, 0); x < xx + w; x++, xxx++)
                    {
                        if (x >= 0 && y >= 0 && x < bW && y < bH && xxx >= 0 && yyy >= 0 && xxx < uW && yyy < uH)
                        {
                            int z = y * strideL + x * 4;
                            int z2 = yyy * strideU + xxx * 4;

                            switch (this.MergeOperation)
                            {
                                case MergeOperation.APlusB:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor(pL[z] + (pU[z2] * pU[z2 + 3] * opc / 255.0));
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor(pL[z + 1] + (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0));
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor(pL[z + 2] + (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0));
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.AMinusB:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor(pL[z] - (pU[z2] * pU[z2 + 3] * opc / 255.0));
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor(pL[z + 1] - (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0));
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor(pL[z + 2] - (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0));
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.BMinusA:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor((pU[z2] * pU[z2 + 3] * opc / 255.0) - pL[z]);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor((pU[z2 + 1] * pU[z2 + 3] * opc / 255.0) - pL[z + 1]);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor((pU[z2 + 2] * pU[z2 + 3] * opc / 255.0) - pL[z + 2]);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.Difference:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor(Math.Abs(pL[z] - (pU[z2] * pU[z2 + 3] * opc / 255.0)));
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor(Math.Abs(pL[z + 1] - (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0)));
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor(Math.Abs(pL[z + 2] - (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0)));
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.APlusBBy2:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor((pL[z] + (pL[z] * ((255 - (pU[z2 + 3] * opc)) / 255.0)) + (pU[z2] * pU[z2 + 3] * opc / 255.0)) / 2.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor((pL[z + 1] + (pL[z + 1] * ((255 - (pU[z2 + 3] * opc)) / 255.0)) + (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0)) / 2.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor((pL[z + 2] + (pL[z + 2] * ((255 - (pU[z2 + 3] * opc)) / 255.0)) + (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0)) / 2.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.Multiply:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor(pL[z] * (pU[z2] * pU[z2 + 3] * opc / 255.0) / 255.0);
                                        blue = Math.Floor(pL[z] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (blue * pU[z2 + 3] / 255.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor(pL[z + 1] * (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0) / 255.0);
                                        green = Math.Floor(pL[z + 1] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (green * pU[z2 + 3] / 255.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor(pL[z + 2] * (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0) / 255.0);
                                        red = Math.Floor(pL[z + 2] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (red * pU[z2 + 3] / 255.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.Screen:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Floor(255.0 - ((255.0 - pL[z]) * (255 - (pU[z2] * pU[z2 + 3] / 255.0)) / 255.0));
                                        blue = Math.Floor(pL[z] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (blue * pU[z2 + 3] / 255.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor(255.0 - ((255.0 - pL[z + 1]) * (255 - (pU[z2 + 1] * pU[z2 + 3] / 255.0)) / 255.0));
                                        green = Math.Floor(pL[z + 1] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (green * pU[z2 + 3] / 255.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor(255.0 - ((255.0 - pL[z + 2]) * (255 - (pU[z2 + 2] * pU[z2 + 3] / 255.0)) / 255.0));
                                        red = Math.Floor(pL[z + 2] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (red * pU[z2 + 3] / 255.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.DarkenOnly:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Min(pL[z], (pU[z2] * pU[z2 + 3] * opc / 255.0));
                                        blue = Math.Floor(pL[z] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (blue * pU[z2 + 3] / 255.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Min(pL[z + 1], (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0));
                                        green = Math.Floor(pL[z + 1] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (green * pU[z2 + 3] / 255.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Min(pL[z + 2], (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0));
                                        red = Math.Floor(pL[z + 2] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (red * pU[z2 + 3] / 255.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.LightenOnly:
                                    if (pU[z2 + 3] > 0)
                                    {
                                        double blue = Math.Max(pL[z], (pU[z2] * pU[z2 + 3] * opc / 255.0));
                                        blue = Math.Floor(pL[z] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (blue * pU[z2 + 3] / 255.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Max(pL[z + 1], (pU[z2 + 1] * pU[z2 + 3] * opc / 255.0));
                                        green = Math.Floor(pL[z + 1] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (green * pU[z2 + 3] / 255.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Max(pL[z + 2], (pU[z2 + 2] * pU[z2 + 3] * opc / 255.0));
                                        red = Math.Floor(pL[z + 2] * (255.0 - pU[z2 + 3]) * opc / 255.0) + (red * pU[z2 + 3] / 255.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.DivideAB:
                                    {
                                        double blue = (pL[z] + 0.0001) / (pU[z2] + 0.0001);
                                        pL[z] = (byte)Math.Max(Math.Min(Math.Pow(255, blue), 255), 0);

                                        double green = (pL[z + 1] + 0.0001) / (pU[z2 + 1] + 0.0001);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(Math.Pow(255, green), 255), 0);

                                        double red = (pL[z + 2] + 0.0001) / (pU[z2 + 2] + 0.0001);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(Math.Pow(255, red), 255), 0);
                                    }
                                    break;

                                case MergeOperation.DivideABStraight:
                                    {
                                        double blue = Math.Floor((pL[z] + 0.0001) / (pU[z2] + 0.0001) * 255.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor((pL[z + 1] + 0.0001) / (pU[z2 + 1] + 0.0001) * 255.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor((pL[z + 2] + 0.0001) / (pU[z2 + 2] + 0.0001) * 255.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.DivideBA:
                                    {
                                        double blue = (pU[z2] + 0.0001) / (pL[z] + 0.0001);
                                        pL[z] = (byte)Math.Max(Math.Min(Math.Pow(255, blue), 255), 0);

                                        double green = (pU[z2 + 1] + 0.0001) / (pL[z + 1] + 0.0001);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(Math.Pow(255, green), 255), 0);

                                        double red = (pU[z2 + 2] + 0.0001) / (pL[z + 2] + 0.0001);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(Math.Pow(255, red), 255), 0);
                                    }
                                    break;

                                case MergeOperation.DivideBAStraight:
                                    {
                                        double blue = Math.Floor((pU[z2] + 0.0001) / (pL[z] + 0.0001) * 255.0);
                                        pL[z] = (byte)Math.Max(Math.Min(blue, 255), 0);

                                        double green = Math.Floor((pU[z2 + 1] + 0.0001) / (pL[z + 1] + 0.0001) * 255.0);
                                        pL[z + 1] = (byte)Math.Max(Math.Min(green, 255), 0);

                                        double red = Math.Floor((pU[z2 + 2] + 0.0001) / (pL[z + 2] + 0.0001) * 255.0);
                                        pL[z + 2] = (byte)Math.Max(Math.Min(red, 255), 0);
                                    }
                                    break;

                                case MergeOperation.AlphaMask_BGForAlphaGr0:
                                    if (pU[z2 + 3] == 0)
                                        pL[z + 3] = (byte)0;
                                    break;

                                case MergeOperation.AlphaMask_AlphaFromMask:
                                    pL[z + 3] = pU[z2 + 3];
                                    break;

                                case MergeOperation.AlphaMask_Invers:
                                    if (pU[z2 + 3] > 0)
                                        pL[z + 3] = (byte)(255 - pU[z2 + 3]);
                                    break;

                                case MergeOperation.AlphaMask_AlphaFromMaskWhenLower:
                                    if (pL[z + 3] > 0)
                                        pL[z + 3] = pU[z2 + 3];
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                });

                //clear the remainig parts of the lower image when alphamasking
                switch (this.MergeOperation)
                {
                    case MergeOperation.AlphaMask_BGForAlphaGr0:
                    case MergeOperation.AlphaMask_AlphaFromMask:
                    //case MergeOperation.AlphaMask_Invers: //switched off, since we usually want to keep the whole orig part.
                    case MergeOperation.AlphaMask_AlphaFromMaskWhenLower:
                        Parallel.For(0, bH, y =>
                        {
                            Rectangle rcDrawTmp = new(rcDraw.Location, rcDraw.Size);
                            byte* p = (byte*)bmL.Scan0;
                            p += y * strideL;
                            for (int x = 0; x < bW; x++)
                            {
                                if (!rcDrawTmp.Contains(x, y))
                                    p[3] = 0;

                                p += 4;
                            }
                        });
                        break;

                    default:
                        break;
                }

                bmpUpper.UnlockBits(bmU);
                background.UnlockBits(bmL);

                //Form fff = new Form();
                //fff.BackgroundImage = background;
                //fff.BackgroundImageLayout = ImageLayout.Zoom;
                //fff.ShowDialog();
            }
        }

        private SizeF GetRotatedSize(float rotation, RectangleF bounds)
        {
            if (rotation == 0f)
                return new SizeF(bounds.Width, bounds.Height);
            else
            {
                double rad = rotation / 180.0 * Math.PI;
                double x = Math.Abs(Math.Cos(rad)) * bounds.Width + Math.Abs(Math.Sin(rad)) * bounds.Height;
                double y = Math.Abs(Math.Sin(rad)) * bounds.Width + Math.Abs(Math.Cos(rad)) * bounds.Height;
                return new SizeF((float)x, (float)y);
            }
        }

        internal GraphicsPath GetCompatibleGraphicsPath()
        {
            RectangleF rc = new RectangleF(this.Bounds.X * this.Zoom,
                this.Bounds.Y * this.Zoom,
                this.Bounds.Width * this.Zoom,
                this.Bounds.Height * this.Zoom);

            GraphicsPath gP = new GraphicsPath();
            gP.AddRectangle(rc);

            if (this.Rotation != 0f)
            {
                using Matrix mx = new Matrix(1f, 0, 0, 1f, 0, 0);
                mx.RotateAt(this.Rotation, new PointF(rc.X, rc.Y));
                gP.Transform(mx);
            }

            return gP;
        }

        internal RectangleF GetCompatibleGraphicsPath(float zoom, SizeF sz)
        {
            RectangleF rc = new RectangleF(this.Bounds.X * zoom,
                this.Bounds.Y * zoom,
                this.Bounds.Width * zoom,
                this.Bounds.Height * zoom);

            GraphicsPath gP = new GraphicsPath();
            gP.AddRectangle(rc);

            if (this.Rotation != 0f)
            {
                using Matrix mx = new Matrix(1f, 0, 0, 1f, 0, 0);
                mx.RotateAt(this.Rotation, new PointF(rc.X, rc.Y));
                gP.Transform(mx);
            }

            RectangleF r4 = gP.GetBounds();

            return r4;
        }

        internal RectangleF GetCompatibleGraphicsPathNoRotation(float zoom, SizeF sz)
        {
            RectangleF rc = new RectangleF(this.Bounds.X * zoom,
                this.Bounds.Y * zoom,
                this.Bounds.Width * zoom,
                this.Bounds.Height * zoom);

            GraphicsPath gP = new GraphicsPath();
            gP.AddRectangle(rc);

            //if (this.Rotation != 0f)
            //{
            //    using Matrix mx = new Matrix(1f, 0, 0, 1f, 0, 0);
            //    mx.RotateAt(this.Rotation, new PointF(rc.X, rc.Y));
            //    gP.Transform(mx);
            //}

            RectangleF r4 = gP.GetBounds();

            return r4;
        }
    }
}
