using HelplineRulerControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetAlphaMatte
{
    public partial class frmSelectTrimap : Form
    {
        private Bitmap? _selectedBmp;
        private HelplineRulerCtrl? _selectedHLC;
        private Bitmap _bmpBU;
        private Bitmap _picOverlay;
        private List<DrawInfo> _allPts = new List<DrawInfo>();
        private List<PointF> _pointsDraw = new List<PointF>();
        private int _drawType = 2;
        private int _iX = 0;
        private int _iY = 0;
        private bool _tracking4;

        public Bitmap? FBitmap
        {
            get
            {
                return this._selectedBmp;
            }
        }

        public frmSelectTrimap(Bitmap b1, Bitmap b2, Bitmap bWork)
        {
            InitializeComponent();

            Bitmap bmp1 = new Bitmap(b1);
            Bitmap bmp2 = new Bitmap(b2);
            this.helplineRulerCtrl1.Bmp = bmp1;
            this.helplineRulerCtrl2.Bmp = bmp2;
            this.helplineRulerCtrl3.Bmp = new Bitmap(bmp1.Width, bmp1.Height);
            using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl3.Bmp);
            gx.Clear(Color.Gray);

            this._bmpBU = new Bitmap(this.helplineRulerCtrl3.Bmp);

            this._picOverlay = bWork;

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.AddDefaultHelplines();
            this.helplineRulerCtrl1.ResetAllHelpLineLabelsColor();

            this.helplineRulerCtrl1.dbPanel1.MouseDown += helplineRulerCtrl1_MouseDown;

            double faktor2 = System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Height);
            double multiplier2 = System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Height);
            if (multiplier2 >= faktor2)
                this.helplineRulerCtrl2.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Width));
            else
                this.helplineRulerCtrl2.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Height));

            this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
            this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

            this.helplineRulerCtrl2.AddDefaultHelplines();
            this.helplineRulerCtrl2.ResetAllHelpLineLabelsColor();

            this.helplineRulerCtrl2.dbPanel1.MouseDown += helplineRulerCtrl2_MouseDown;

            double faktor3 = System.Convert.ToDouble(helplineRulerCtrl3.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl3.dbPanel1.Height);
            double multiplier3 = System.Convert.ToDouble(this.helplineRulerCtrl3.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl3.Bmp.Height);
            if (multiplier3 >= faktor3)
                this.helplineRulerCtrl3.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl3.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl3.Bmp.Width));
            else
                this.helplineRulerCtrl3.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl3.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl3.Bmp.Height));

            this.helplineRulerCtrl3.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl3.Bmp.Width * this.helplineRulerCtrl3.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl3.Bmp.Height * this.helplineRulerCtrl3.Zoom));
            this.helplineRulerCtrl3.MakeBitmap(this.helplineRulerCtrl3.Bmp);

            this.helplineRulerCtrl3.AddDefaultHelplines();
            this.helplineRulerCtrl3.ResetAllHelpLineLabelsColor();

            this.helplineRulerCtrl3.dbPanel1.MouseDown += helplineRulerCtrl3_MouseDown;
            this.helplineRulerCtrl3.dbPanel1.MouseMove += helplineRulerCtrl3_MouseMove;
            this.helplineRulerCtrl3.dbPanel1.MouseUp += helplineRulerCtrl3_MouseUp;

            this.helplineRulerCtrl3.PostPaint += helplineRulerCtrl3_Paint;

            this.helplineRulerCtrl3.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            SelectHLC(this.helplineRulerCtrl1);
        }

        private void helplineRulerCtrl2_MouseDown(object? sender, MouseEventArgs e)
        {
            SelectHLC(this.helplineRulerCtrl2);
        }

        private void helplineRulerCtrl3_MouseDown(object? sender, MouseEventArgs e)
        {
            SelectHLC(this.helplineRulerCtrl3);

            if (this.helplineRulerCtrl3.Bmp != null && this.cbDraw.Checked)
            {
                this._iX = (int)((e.X - this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl3.Zoom);
                this._iY = (int)((e.Y - this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl3.Zoom);

                this.helplineRulerCtrl3.dbPanel1.Capture = true;

                if (this._iX >= 0 && this._iX < this.helplineRulerCtrl3.Bmp.Width && this._iY >= 0 && this._iY < this.helplineRulerCtrl3.Bmp.Height)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this._pointsDraw.Clear();
                        this._pointsDraw.Add(new Point(this._iX, this._iY));
                        this._tracking4 = true;
                    }
                }
            }
        }

        private void SelectHLC(HelplineRulerCtrl hlc)
        {
            this._selectedBmp = hlc.Bmp;
            this._selectedHLC = hlc;
            this.helplineRulerCtrl1.SetRulersBackColor(SystemColors.ControlDark);
            this.helplineRulerCtrl2.SetRulersBackColor(SystemColors.ControlDark);
            this.helplineRulerCtrl3.SetRulersBackColor(SystemColors.ControlDark);
            hlc.SetRulersBackColor(Color.GreenYellow);

            this.label2.Text = "selected Bmp: " + hlc.Name;

            this.btnOK.Enabled = true;
        }

        private void helplineRulerCtrl3_MouseMove(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl3.Zoom);
            int iy = (int)((e.Y - this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl3.Zoom);

            if (ix >= this.helplineRulerCtrl3.Bmp.Width)
                ix = this.helplineRulerCtrl3.Bmp.Width - 1;
            if (iy >= this.helplineRulerCtrl3.Bmp.Height)
                iy = this.helplineRulerCtrl3.Bmp.Height - 1;

            if (ix >= 0 && ix < this.helplineRulerCtrl3.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl3.Bmp.Height)
            {
                if (this._tracking4)
                {
                    if (!this.cbClickMode.Checked || this._pointsDraw.Count < 2)
                        this._pointsDraw.Add(new Point(ix, iy));
                    else
                        this._pointsDraw[this._pointsDraw.Count - 1] = new Point(ix, iy);
                    this.helplineRulerCtrl3.dbPanel1.Invalidate();
                }
            }
        }

        private void helplineRulerCtrl3_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl3.Bmp != null && this.cbDraw.Checked)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl3.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl3.Zoom);

                if (ix >= this.helplineRulerCtrl3.Bmp.Width)
                    ix = this.helplineRulerCtrl3.Bmp.Width - 1;
                if (iy >= this.helplineRulerCtrl3.Bmp.Height)
                    iy = this.helplineRulerCtrl3.Bmp.Height - 1;

                if (ix >= 0 && ix < this.helplineRulerCtrl3.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl3.Bmp.Height)
                {
                    if (this._tracking4)
                    {
                        if (this._iX != ix && this._iY != iy)
                            this._pointsDraw.Add(new Point(ix, iy));
                        DrawToBitmap();
                    }

                    this.helplineRulerCtrl3.dbPanel1.Invalidate();
                }
                else
                {
                    if (this._tracking4)
                        DrawToBitmap();

                    this.helplineRulerCtrl3.dbPanel1.Invalidate();
                }
            }

            this._tracking4 = false;
            this.helplineRulerCtrl3.dbPanel1.Capture = false;
        }

        private void DrawToBitmap()
        {
            if (this._allPts != null && this._pointsDraw != null && ((this._allPts.Count > 0) || (this._pointsDraw.Count > 0)))
            {
                Bitmap bDraw = new Bitmap(this._bmpBU);

                using Graphics gx = Graphics.FromImage(bDraw);
                gx.PixelOffsetMode = PixelOffsetMode.Half;
                gx.SmoothingMode = SmoothingMode.None;
                gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                if (this._pointsDraw.Count > 0)
                {
                    List<PointF> points = new List<PointF>();
                    points.AddRange(this._pointsDraw);
                    DrawInfo di = new DrawInfo()
                    {
                        Points = points,
                        DrawWidth = (float)this.numWHScribbles.Value,
                        DrawCol = this.rbBG.Checked ? Color.Black : (this.rbFG.Checked ? Color.White : Color.Gray)
                    };

                    this._allPts.Add(di);

                    this._pointsDraw.Clear();
                }

                for (int j = 0; j < this._allPts.Count; j++)
                {
                    DrawInfo di = this._allPts[j];

                    if (di.Points != null && di.Points.Count > 1)
                    {
                        using GraphicsPath gP = new();
                        gP.AddLines(di.Points.ToArray());

                        using Pen pen = new Pen(di.DrawCol, di.DrawWidth);
                        pen.LineJoin = LineJoin.Round;
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        gx.DrawPath(pen, gP);
                    }
                    else if (di.Points != null && di.Points.Count == 1)
                    {
                        using SolidBrush sb = new SolidBrush(di.DrawCol);
                        gx.FillEllipse(sb, new RectangleF(di.Points[0].X - di.DrawWidth / 2f, di.Points[0].Y - di.DrawWidth / 2f, di.DrawWidth, di.DrawWidth));
                    }
                }

                this.SetBitmap(this.helplineRulerCtrl3.Bmp, bDraw, this.helplineRulerCtrl3, "Bmp");
                this.helplineRulerCtrl3.MakeBitmap(this.helplineRulerCtrl3.Bmp);
                this.helplineRulerCtrl3.dbPanel1.Invalidate();
            }
            else if (this._allPts != null && this._pointsDraw != null && this._allPts.Count == 0 && this._pointsDraw.Count == 0)
            {
                Bitmap bDraw = new Bitmap(this._bmpBU);
                this.SetBitmap(this.helplineRulerCtrl3.Bmp, bDraw, this.helplineRulerCtrl3, "Bmp");
                this.helplineRulerCtrl3.MakeBitmap(this.helplineRulerCtrl3.Bmp);
                this.helplineRulerCtrl3.dbPanel1.Invalidate();
            }
        }

        private void SetBitmap(Bitmap? bitmapToSet, Bitmap? bitmapToBeSet, Control ct, string property)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();

            if (ct != null)
            {
                if (ct.GetType().GetProperties().Where((a) => a.Name == property).Count() > 0)
                {
                    System.Reflection.PropertyInfo? pi = ct.GetType().GetProperty(property);
                    pi?.SetValue(ct, bitmapToBeSet);
                }
            }
        }

        private void helplineRulerCtrl3_Paint(object sender, PaintEventArgs e)
        {
            if (this._picOverlay != null)
            {
                HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl3.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.55F;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);
                    e.Graphics.DrawImage(this._picOverlay,
                        new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                        -pz.AutoScrollPosition.X / this.helplineRulerCtrl3.Zoom,
                            -pz.AutoScrollPosition.Y / this.helplineRulerCtrl3.Zoom,
                            pz.ClientRectangle.Width / this.helplineRulerCtrl3.Zoom,
                            pz.ClientRectangle.Height / this.helplineRulerCtrl3.Zoom, GraphicsUnit.Pixel, ia);
                }
            }

            if (this.helplineRulerCtrl3.Bmp != null && this.cbDraw.Checked)
            {
                if (this._pointsDraw != null && this._pointsDraw.Count > 0)
                {
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    e.Graphics.SmoothingMode = SmoothingMode.None;
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                    if (this._pointsDraw.Count > 1)
                    {
                        using GraphicsPath gP = new GraphicsPath();
                        gP.AddLines(this._pointsDraw.ToArray());

                        using Matrix mx = new Matrix(this.helplineRulerCtrl3.Zoom, 0, 0, this.helplineRulerCtrl3.Zoom,
                            this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl3.dbPanel1.AutoScrollPosition.Y);

                        gP.Transform(mx);

                        using Pen pen = new Pen(this.rbBG.Checked ? Color.Black : (this.rbFG.Checked ? Color.White : Color.Gray),
                            Math.Max((float)this.numWHScribbles.Value * this.helplineRulerCtrl3.Zoom, 1f));
                        pen.LineJoin = LineJoin.Round;
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        e.Graphics.DrawPath(pen, gP);
                    }
                    else if (this._pointsDraw.Count == 1)
                    {
                        using SolidBrush sb = new SolidBrush(this.rbBG.Checked ? Color.Black : (this.rbFG.Checked ? Color.White : Color.Gray));
                        float wh = Math.Max((float)this.numWHScribbles.Value * this.helplineRulerCtrl3.Zoom, 1f);
                        e.Graphics.FillEllipse(sb, new RectangleF(this._pointsDraw[0].X - wh / 2f,
                            this._pointsDraw[0].Y - wh / 2f, wh, wh));
                    }
                }
            }
        }

        private void frmSelectTrimap_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._bmpBU != null)
                this._bmpBU.Dispose();
        }

        private void rbBG_CheckedChanged(object sender, EventArgs e)
        {
            this._drawType = 0;
        }

        private void rbFG_CheckedChanged(object sender, EventArgs e)
        {
            this._drawType = 1;
        }

        private void rbUnknown_CheckedChanged(object sender, EventArgs e)
        {
            this._drawType = 2;
        }

        private void btnRemLastScribbles_Click(object sender, EventArgs e)
        {
            if (this._allPts != null && this._allPts.Count > 0)
                this._allPts.RemoveAt(this._allPts.Count - 1);

            this.DrawToBitmap();
        }

        private void btnClearScribbles_Click(object sender, EventArgs e)
        {
            if (this._pointsDraw != null && this._pointsDraw.Count > 0)
                this._pointsDraw.Clear();

            if (this._allPts != null && this._allPts.Count > 0)
            {
                this._allPts.Clear();
                this.DrawToBitmap();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._selectedBmp = this._selectedHLC?.Bmp;
        }
    }
}
