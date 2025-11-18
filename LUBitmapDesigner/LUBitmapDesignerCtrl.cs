using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;

namespace LUBitmapDesigner
{
    public partial class LUBitmapDesignerCtrl : UserControl
    {
        private int _ix;
        private int _iy;
        private float _rotationStart;
        public BitmapShape? SelectedShape { get; set; }
        public PointF CurPt { get; private set; }
        public bool ShadowMode { get; set; }

        public ShapeList? ShapeList { get; set; }

        public event EventHandler<BitmapShape>? ShapeChanged;
        public event EventHandler<BitmapShape>? ShapeRemoved;
        public event EventHandler<bool>? SPC;

        private Bitmap? _bgImage;
        private bool _mouseIsDown;

        //private bool _drawing;

        public LUBitmapDesignerCtrl()
        {
            InitializeComponent();
        }

        public void SetupBGImage()
        {
            //draw bgImage and call hlc_MakeBitmap(bgImage), then draw the top shape in postpaint
            if (/*!this._drawing &&*/ this.ShapeList != null && this.ShapeList.Count > 0)
            {
                //this._drawing = true;

                using Bitmap bmp = new(this.helplineRulerCtrl1.Bmp);

                for (int i = 1; i < this.ShapeList.Count; i++)
                {
                    if (this.SelectedShape != null && !this.ShapeList[i].Equals(this.SelectedShape))
                    {   
                        float zoom = this.ShapeList[i].Zoom;
                        this.ShapeList[i].Zoom = 1.0F;
                        this.ShapeList[i].Draw(bmp);
                        this.ShapeList[i].Zoom = zoom;
                    }
                }

                this.helplineRulerCtrl1.MakeBitmap(bmp);

                if (this.helplineRulerCtrl1.BmpTmp != null)
                {
                    Bitmap? bOld = this._bgImage;
                    this._bgImage = (Bitmap)this.helplineRulerCtrl1.BmpTmp.Clone();
                    if (bOld != null)
                        bOld.Dispose();

                    bOld = null;
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();               
            } 
            
            //this._drawing = false;
        }

        public void SetBlankLowerImage()
        {
            Bitmap bLower = new Bitmap(this.Width, this.Height);
            if (this.ShapeList == null)
                this.ShapeList = this.ShadowMode ? new ShadowShapeList() : new ShapeList();
            BitmapShape b = new BitmapShape() { Bmp = bLower, Bounds = new RectangleF(0, 0, bLower.Width, bLower.Height), Rotation = 0, Zoom = 1f };
            this.ShapeList.Add(b);

            this.helplineRulerCtrl1.Bmp = bLower;
        }

        public Bitmap? GetUpperImage()
        {
            Bitmap? bOut = null;
            if (this.SelectedShape != null)
                bOut = this.SelectedShape.Bmp;
            else
            {
                if (this.ShapeList == null)
                    this.ShapeList = this.ShadowMode ? new ShadowShapeList() : new ShapeList();
                if (this.ShapeList.Count == 0)
                    SetBlankLowerImage();
                if (this.ShapeList.Count > 2)
                {
                    bOut = this.ShapeList[2].Bmp;
                    this.SelectedShape = this.ShapeList[2];
                }
                else if (this.ShapeList.Count > 1)
                {
                    bOut = this.ShapeList[1].Bmp;
                    this.SelectedShape = this.ShapeList[1];
                }
            }

            return bOut;
        }

        public void SetUpperImage(Bitmap bmp, float x, float y)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                if (this.ShapeList == null)
                    this.ShapeList = this.ShadowMode ? new ShadowShapeList() : new ShapeList();
                if (this.ShapeList.Count == 0)
                    SetBlankLowerImage();

                if (bmp != null && this.ShapeList.Count > 0)
                {
                    if (this.ShapeList is ShadowShapeList)
                    {
                        for (int i = 3; i < this.ShapeList.Count; i++)
                            this.ShapeList.RemoveAt(i);

                        int cnt = this.ShapeList.Count;

                        BitmapShape? b = null;
                        if (this.ShapeList.Count > 1)
                            b = this.ShapeList[this.ShapeList.Count - 1];

                        RectangleF? r = null;
                        float? rot = null;
                        //old location and size
                        if (b != null)
                        {
                            r = b.Bounds;
                            rot = b.Rotation;
                        }

                        RectangleF rc = new RectangleF(x, y, bmp.Width, bmp.Height);

                        this.ShapeList.AllowAdding = true;
                        this.ShapeList.Add(new BitmapShape() { Bmp = bmp, Bounds = rc, Rotation = 0, Zoom = 1f });
                        if (r != null && rot != null)
                        {
                            this.ShapeList[this.ShapeList.Count - 1].Bounds = r.Value;
                            this.ShapeList[this.ShapeList.Count - 1].Rotation = rot.Value;
                        }

                        if (cnt >= 2)
                            this.ShapeList.RemoveAt(cnt - 1);

                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }
                    }
                    else
                    {
                        for (int i = 2; i < this.ShapeList.Count; i++)
                            this.ShapeList.RemoveAt(i);

                        int cnt = this.ShapeList.Count;

                        BitmapShape? b = null;
                        if (this.ShapeList.Count > 1)
                            b = this.ShapeList[1];

                        RectangleF? r = null;
                        float? rot = null;
                        //old location and size
                        if (b != null)
                        {
                            r = b.Bounds;
                            rot = b.Rotation;
                        }

                        RectangleF rc = new RectangleF(x, y, bmp.Width, bmp.Height);

                        this.ShapeList.AllowAdding = true;
                        this.ShapeList.Add(new BitmapShape() { Bmp = bmp, Bounds = rc, Rotation = 0, Zoom = 1f });
                        if (r != null && rot != null)
                        {
                            this.ShapeList[this.ShapeList.Count - 1].Bounds = r.Value;
                            this.ShapeList[this.ShapeList.Count - 1].Rotation = rot.Value;
                        }

                        if (cnt >= 2)
                            this.ShapeList.RemoveAt(1);

                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }

            this.helplineRulerCtrl1.Bmp = this.ShapeList[0].Bmp;

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp?.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp?.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp?.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp?.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp?.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp?.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
        }

        public void AddUpperImage(Bitmap bmp)
        {
            if (this.ShapeList != null)
            {
                for (int i = 3; i < this.ShapeList.Count; i++)
                    this.ShapeList.RemoveAt(i);

                int cnt = this.ShapeList.Count;

                BitmapShape? b = null;
                if (this.ShapeList.Count > 1)
                    b = this.ShapeList[this.ShapeList.Count - 1];

                RectangleF? r = null;
                float? rot = null;
                //old location and size
                if (b != null)
                {
                    r = b.Bounds;
                    rot = b.Rotation;
                }

                RectangleF rc = new RectangleF(0, 0, bmp.Width, bmp.Height);

                //this.ShapeList.AllowAdding = true;
                this.ShapeList.Add(new BitmapShape() { Bmp = bmp, Bounds = rc, Rotation = 0, Zoom = 1f });
            }
        }

        private void helplineRulerCtrl1_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.helplineRulerCtrl1.AddDefaultHelplines();
                this.helplineRulerCtrl1.ResetAllHelpLineLabelsColor();

                this.helplineRulerCtrl1.dbPanel1.MouseDown += helplineRulerCtrl1_MouseDown;
                this.helplineRulerCtrl1.dbPanel1.MouseMove += helplineRulerCtrl1_MouseMove;
                this.helplineRulerCtrl1.dbPanel1.MouseUp += helplineRulerCtrl1_MouseUp;

                this.helplineRulerCtrl1.PostPaint += helplineRulerCtrl1_Paint;
            }
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.ShapeList?.Count > 1 && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                this.SelectedShape = null;
                this.SPC?.Invoke(this, false);

                if (this.ShapeList.Count > 1 && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
                {
                    int id = HitTest(ix, iy);

                    if (id > -1)
                    {
                        this._ix = ix;
                        this._iy = iy;

                        BitmapShape b = this.ShapeList.Shapes.Where(a => a.ID == id).First();
                        this.SelectedShape = b;
                        SetupBGImage();
                        this.CurPt = new PointF(ix - b.Bounds.X, iy - b.Bounds.Y);
                        this.SPC?.Invoke(this, true);
                        this._mouseIsDown = true;

                        if (e.Button == MouseButtons.Right && this.SelectedShape?.IsLocked == false)
                        {
                            float xP1 = ix - b.Bounds.X;
                            float yP1 = iy - b.Bounds.Y;

                            _rotationStart = (float)(Math.Atan2(yP1, xP1) / (Math.PI / 180.0)) - b.Rotation;
                        }
                    }
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.ShapeList?.Count > 1 && e.Button == MouseButtons.Left)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                //int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                //int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this.SelectedShape != null && this.SelectedShape?.IsLocked == false)
                    {
                        this.SelectedShape.Bounds = new RectangleF(this.SelectedShape.Bounds.X + (ix - _ix), this.SelectedShape.Bounds.Y + (iy - _iy), this.SelectedShape.Bounds.Width, this.SelectedShape.Bounds.Height);
                        this._ix = ix;
                        this._iy = iy;
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        ShapeChanged?.Invoke(this, this.SelectedShape);
                    }
                }
            }
            if (this.ShapeList?.Count > 1 && e.Button == MouseButtons.Right)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this.SelectedShape != null && this.SelectedShape?.IsLocked == false)
                    {
                        PointF f = this.SelectedShape.Bounds.Location;

                        float pX = ix - f.X;
                        float pY = iy - f.Y;

                        this.SelectedShape.Rotation = (float)(Math.Atan2(pY, pX) / (Math.PI / 180.0)) - _rotationStart;

                        if (this.SelectedShape.Rotation > 360.0F)
                            this.SelectedShape.Rotation -= 360;

                        if (this.SelectedShape.Rotation < -360.0F)
                            this.SelectedShape.Rotation += 360;

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        ShapeChanged?.Invoke(this, this.SelectedShape);
                    }
                }
            }
        }

        private int HitTest(int ix, int iy)
        {
            // check, if a DrawnRectangle is at the current point
            if (this.ShapeList != null && this.ShapeList.Count > 1)
            {
                for (int i = this.ShapeList.Count - 1; i > 0; i--)
                {
                    // we use a GraphicsPath for testing, because it is easy to handle when obects are rotated
                    // then, we simply rotate the path
                    using (GraphicsPath gP = new GraphicsPath())
                    {
                        gP.FillMode = FillMode.Winding;

                        using (Matrix m = new Matrix(1, 0, 0, 1, 0, 0))
                        {
                            gP.AddRectangle(this.ShapeList[i].Bounds);

                            // setup the matrix
                            if (this.ShapeList[i].Rotation != 0)
                                m.RotateAt(this.ShapeList[i].Rotation, this.ShapeList[i].Bounds.Location, MatrixOrder.Prepend);

                            // and transform (here: rotate) the path
                            gP.Transform(m);
                        }

                        PointF p = new PointF(ix, iy);

                        // with the method "IsVisible" we check, if the current point is inside the path
                        if (gP.IsVisible(p))
                        {
                            //if we hit the lower shape, bring it to front
                            if (this.ShapeList.Count > 2 && i != this.ShapeList.Count - 1) //if we only have one shape and the background, the upper shape is always shaapelist.count - 1, so the first check isnt really needed
                            {
                                ShadowShapeList sl = (ShadowShapeList)this.ShapeList;
                                sl.SwapUpperShapes();

                                int j = this.ShapeList.Count - 1;
                                ShapeChanged?.Invoke(this, this.ShapeList[j]);
                                gP.Dispose();
                                return this.ShapeList[j].ID;
                            }
                            else
                            {
                                ShapeChanged?.Invoke(this, this.ShapeList[i]);
                                gP.Dispose();
                                return this.ShapeList[i].ID;
                            }
                        }
                    }
                }
            }

            return -1;
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.ShapeList?.Count > 1 && e.Button == MouseButtons.Left && this.SelectedShape != null && this.SelectedShape?.IsLocked == false)
                ShapeChanged?.Invoke(this, this.SelectedShape);

            this._mouseIsDown = false;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void helplineRulerCtrl1_Paint(object? sender, PaintEventArgs e)
        {
            if (this.ShapeList?.Count > 1)
            {
                e.Graphics.SetClip(new Rectangle(0, 0, this.helplineRulerCtrl1.dbPanel1.ClientSize.Width, this.helplineRulerCtrl1.dbPanel1.ClientSize.Height));

                if (this._bgImage != null)
                {
                    using Bitmap bmp = (Bitmap)this._bgImage.Clone();
                    if (this.ShapeList.Count > 1 && this.SelectedShape != null && !this._mouseIsDown)
                    {
                        this.SelectedShape.Draw(bmp);
                        e.Graphics.Clear(this.helplineRulerCtrl1.dbPanel1.BackColor); //needed for Alphamasks
                        e.Graphics.DrawImageUnscaled(bmp, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    }
                    else if(this.ShapeList.Count > 1 && this.SelectedShape == null)
                    {
                        this.ShapeList[this.ShapeList.Count - 1].Draw(bmp);
                        e.Graphics.Clear(this.helplineRulerCtrl1.dbPanel1.BackColor); //needed for Alphamasks
                        e.Graphics.DrawImageUnscaled(bmp, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    }

                    e.Graphics.TranslateTransform(this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);

                    e.Graphics.ResetClip();

                    if (this.SelectedShape != null)
                    {
                        RectangleF rc = new RectangleF(this.SelectedShape.Bounds.X * this.helplineRulerCtrl1.Zoom,
                          this.SelectedShape.Bounds.Y * this.helplineRulerCtrl1.Zoom,
                          this.SelectedShape.Bounds.Width * this.helplineRulerCtrl1.Zoom,
                          this.SelectedShape.Bounds.Height * this.helplineRulerCtrl1.Zoom);

                        using GraphicsPath gP = new GraphicsPath();
                        gP.AddRectangle(rc);

                        if (this.SelectedShape.Rotation != 0f)
                        {
                            using Matrix mx = new Matrix(1f, 0, 0, 1f, 0, 0);
                            mx.RotateAt(this.SelectedShape.Rotation, new PointF(rc.X, rc.Y));
                            gP.Transform(mx);
                        }

                        e.Graphics.SetClip(gP);
                        using Pen pen = new Pen(Color.Red, 2);
                        e.Graphics.DrawPath(pen, gP);
                        e.Graphics.ResetClip();
                    }

                    e.Graphics.ResetTransform();
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.helplineRulerCtrl1.dbPanel1 != null)
            {
                if (keyData == Keys.Down)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y + 4);
                    return true;
                }
                if (keyData == Keys.Up)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y - 4);
                    return true;
                }
                if (keyData == Keys.Left)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X - 4, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.Right)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X + 4, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    return true;
                }

                if (keyData == Keys.PageDown)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y + this.helplineRulerCtrl1.dbPanel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.PageUp)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y - this.helplineRulerCtrl1.dbPanel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.Home)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-Int16.MaxValue, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.End)
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(Int16.MaxValue, -this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    return true;
                }

                if (keyData == (Keys.Home | Keys.Control))
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(-Int16.MaxValue, -Int16.MaxValue);
                    return true;
                }
                if (keyData == (Keys.End | Keys.Control))
                {
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition = new Point(Int16.MaxValue, Int16.MaxValue);
                    return true;
                }

                if (this.SelectedShape != null && keyData == Keys.Delete)
                {
                    //this.ShapeList?.Remove(this.SelectedShape);
                    //this.SelectedShape = null;
                    //this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.ShapeRemoved?.Invoke(this, this.SelectedShape);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        public void SetZoom(string? zoomString)
        {
            if (this.helplineRulerCtrl1.Bmp != null && zoomString != null)
            {
                float zoom = this.helplineRulerCtrl1.Zoom;

                if (zoomString == "1")
                    zoom = 1.0F;
                else if (zoomString.ToLower() == "fit_width")
                    zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.ClientSize.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                else if (zoomString.ToLower() == "fit")
                {
                    double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                    double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                    if (multiplier >= faktor)
                        zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                    else
                        zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));
                }
                else
                {
                    float j = 0;

                    if (float.TryParse(zoomString, out j))
                    {
                        this.Cursor = Cursors.WaitCursor;
                        zoom = j;
                    }
                }

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * zoom));
                this.helplineRulerCtrl1.Zoom = zoom;

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.Cursor = Cursors.Default;

                if (this.ShapeList != null)
                    for (int i = 0; i < this.ShapeList.Count; i++)
                        this.ShapeList[i].Zoom = zoom;
            }
        }

        public void SetSelectedImage(Bitmap bmp, float x, float y)
        {
            if (this.SelectedShape != null)
            {
                this.SelectedShape.Bmp = bmp;
                this.SelectedShape.Bounds = new RectangleF(x, y, this.SelectedShape.Bounds.Width, this.SelectedShape.Bounds.Height);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            for (int i = 0; i < this.ShapeList?.Count; i++)
            {
                if (this.ShapeList[i].Zoom != e.Zoom)
                    SetZoom(e.Zoom.ToString());
            }

            SetupBGImage();
        }

        public void DisposeBGImage()
        {
            if (this._bgImage != null)
                this._bgImage.Dispose();
            this._bgImage = null;
        }
    }
}
