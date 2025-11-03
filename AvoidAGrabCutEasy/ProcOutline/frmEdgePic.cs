using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetAlphaMatte
{
    public partial class frmEdgePic : Form
    {
        private Bitmap? _picOverlay;

        public Size? BaseSize { get; private set; }
        public Point ClickedPoint { get; internal set; }
        public bool DrawFrame { get; private set; }

        public frmEdgePic(Image image)
        {
            InitializeComponent();

            this.helplineRulerCtrl1.Bmp = (Bitmap)image.Clone();

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.dbPanel1.MouseDown += HelplineRulerCtrl1_MouseDown;
            this.helplineRulerCtrl1.PostPaint += HelplineRulerCtrl1_PostPaint;
        }

        public frmEdgePic(Bitmap image, Bitmap overLay)
        {
            InitializeComponent();

            this._picOverlay = overLay;

            this.helplineRulerCtrl1.Bmp = (Bitmap)image.Clone();

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.dbPanel1.MouseDown += HelplineRulerCtrl1_MouseDown;
            this.helplineRulerCtrl1.PostPaint += HelplineRulerCtrl1_PostPaint;
        }

        private void HelplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                this.ClickedPoint = new Point(ix, iy);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.btnClose.Enabled = true;
            }
        }

        private void HelplineRulerCtrl1_PostPaint(object sender, PaintEventArgs e)
        {
            if (this._picOverlay != null)
            {
                HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.65F;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);
                    e.Graphics.DrawImage(this._picOverlay,
                        new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                        -pz.AutoScrollPosition.X / this.helplineRulerCtrl1.Zoom,
                            -pz.AutoScrollPosition.Y / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Width / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Height / this.helplineRulerCtrl1.Zoom, GraphicsUnit.Pixel, ia);
                }
            }

            if (this.DrawFrame)
            {
                float w = this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom;
                float h = this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom;
                using Pen p = new Pen(Color.LightGray, 4);
                e.Graphics.DrawRectangle(p, new RectangleF(this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y, w, h));
            }

            if(this.ClickedPoint.X > -1 && this.ClickedPoint.Y > -1)
            {
                e.Graphics.FillEllipse(Brushes.Red, new RectangleF(
                    (int)(this.ClickedPoint.X * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X - 4,
                    (int)(this.ClickedPoint.Y * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y - 4,
                    8, 8));
            }
        }

        public frmEdgePic(Image image, Size sz)
        {
            InitializeComponent();

            this.BaseSize = sz;

            this.helplineRulerCtrl1.Bmp = (Bitmap)image.Clone();

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.dbPanel1.MouseDown += HelplineRulerCtrl1_MouseDown;
            this.helplineRulerCtrl1.PostPaint += HelplineRulerCtrl1_PostPaint;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmEdgePic_Load(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.AddDefaultHelplines();
            this.helplineRulerCtrl1.ResetAllHelpLineLabelsColor();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                try
                {
                    if (this.BaseSize != null)
                    {
                        double resPic = (double)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height) /
                                (double)Math.Max(this.BaseSize.Value.Width, this.BaseSize.Value.Height);

                        if (resPic != 1.0 && MessageBox.Show("Resize Trimap?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Bitmap bmp = new Bitmap(this.BaseSize.Value.Width, this.BaseSize.Value.Height);
                            using (Graphics graphics = Graphics.FromImage(bmp))
                            {
                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                                graphics.DrawImage(this.helplineRulerCtrl1.Bmp, 0, 0, bmp.Width, bmp.Height);
                            }
                            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                                bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (this.helplineRulerCtrl1.Bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                            this.helplineRulerCtrl1.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (this.helplineRulerCtrl1.Bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                        this.helplineRulerCtrl1.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        internal void SetupWithFrameAndOKButton()
        {
            this.btnCancel.Visible = this.btnCancel.Enabled = true;
            this.btnClose.Text = "OK";
            this.btnClose.Enabled = false;
            Point pt = this.btnClose.Location;
            this.btnClose.Location = this.btnCancel.Location;
            this.btnCancel.Location = pt;
            this.btnClose.DialogResult = DialogResult.OK;
            this.label1.Visible = true;
            this.Text = "Please click a foreground-Point in the Picture.";

            this.DrawFrame = true;
        }
    }
}
