using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUBitmapDesigner
{
    public class BitmapShape : Shape, IDisposable
    {
        public override RectangleF Bounds { get; set; }
        public override float Rotation { get; set; }
        public override float Zoom { get; set; } = 1.0f;
        public Bitmap? Bmp { get; set; }
        public float Opacity { get; set; } = 1.0f;
        public override int ID { get; set; }
        public Bitmap? OrigBmp { get; set; }
        public bool IsLocked { get; set; }

        public BitmapShape()
        {
            this.ID = this.GetNewId();
        }

        //public BitmapShape(Bitmap origBmp)
        //{
        //    this.ID = this.GetNewId();
        //    this.OrigBmp = new Bitmap(origBmp);
        //}

        private int GetNewId()
        {
            int c = BitmapShape.Cnt;
            BitmapShape.Cnt++;
            return c;
        }

        public override void Draw(Graphics gx)
        {
            if (this.Bmp != null)
            {
                if (this.Zoom == 0f)
                    this.Zoom = 1f;

                RectangleF boundsZ = new RectangleF(this.Bounds.X * this.Zoom, 
                    this.Bounds.Y * this.Zoom, 
                    this.Bounds.Width * this.Zoom, 
                    this.Bounds.Height * this.Zoom);

                GraphicsContainer con = gx.BeginContainer();

                Matrix mx = gx.Transform;

                if (this.Rotation != 0f)
                    mx.RotateAt(this.Rotation, new PointF(this.Bounds.X * this.Zoom, this.Bounds.Y * this.Zoom));

                bool dI = this.CheckDrawInt();

                //if (dI)
                //    mx.Scale(1.04f, 1.04f); //.RotateAt(0.1f, new PointF(0f, 0f));

                gx.Transform = mx;
                gx.SetClip(boundsZ);

                if (this.Opacity == 1.0f)
                {
                    if (this.Rotation == 0f && dI)
                    {
                        //gx.DrawImage(this.Bmp, (int)(this.Bounds.X * this.Zoom),
                        //    (int)(this.Bounds.Y * this.Zoom),
                        //    (int)(this.Bounds.Width * this.Zoom),
                        //    (int)(this.Bounds.Height * this.Zoom));  
                        
                        gx.DrawImage(this.Bmp, (int)boundsZ.X,
                            (int)boundsZ.Y,
                            (int)boundsZ.Width,
                            (int)boundsZ.Height);
                    }
                    else
                        //gx.DrawImage(this.Bmp, this.Bounds.X * this.Zoom,
                        //    this.Bounds.Y * this.Zoom,
                        //    this.Bounds.Width * this.Zoom,
                        //    this.Bounds.Height * this.Zoom);    
                        gx.DrawImage(this.Bmp, boundsZ);
                }
                else
                {
                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = this.Opacity;

                    using (ImageAttributes ia = new ImageAttributes())
                    {
                        ia.SetColorMatrix(cm);
                        gx.DrawImage(this.Bmp,
                            new PointF[] { new PointF(this.Bounds.X * this.Zoom, this.Bounds.Y * this.Zoom),
                            new PointF((this.Bounds.X + this.Bounds.Width) * this.Zoom, this.Bounds.Y * this.Zoom),
                            new PointF(this.Bounds.X * this.Zoom, (this.Bounds.Y + this.Bounds.Height) * this.Zoom) },
                                new RectangleF(0, 0,
                                    this.Bmp.Width, this.Bmp.Height),
                                    GraphicsUnit.Pixel, ia);
                    }
                }

                gx.ResetClip();
                gx.EndContainer(con);
            }
        }

        private bool CheckDrawInt()
        {
            return ((this.Bounds.X * this.Zoom - (int)this.Bounds.X * this.Zoom == 0) &&
                 (this.Bounds.Y * this.Zoom - (int)this.Bounds.Y * this.Zoom == 0) &&
                 (this.Bounds.Width * this.Zoom - (int)this.Bounds.Width * this.Zoom == 0) &&
                 (this.Bounds.Height * this.Zoom - (int)this.Bounds.Height * this.Zoom == 0));
        }

        public override void Dispose()
        {
            if (this.Bmp != null)
            {
                this.Bmp.Dispose();
                this.Bmp = null;
            }
            if (this.OrigBmp != null)
            {
                this.OrigBmp.Dispose();
                this.OrigBmp = null;
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
    }
}
