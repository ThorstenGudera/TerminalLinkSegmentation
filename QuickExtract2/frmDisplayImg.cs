using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuickExtract2
{
    public partial class frmDisplayImg : Form
    {
        private Color _highlightColor = Color.Lime;
        public GraphicsPath CurPath { get; set; }
        public GraphicsPath CurPath2 { get; set; }
        private RectangleF _r2;
        private RectangleF _r;
        private bool _dontDoUpdateNums;

        public frmDisplayImg(Bitmap bmp, GraphicsPath curPath)
        {
            InitializeComponent();

            this.HelplineRulerCtrl1.Bmp = new Bitmap(bmp);

            // Me.HelplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            double faktor = System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Width));
            else
                this.HelplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(HelplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.HelplineRulerCtrl1.Bmp.Height));

            this.HelplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Width * this.HelplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.HelplineRulerCtrl1.Bmp.Height * this.HelplineRulerCtrl1.Zoom));
            this.HelplineRulerCtrl1.MakeBitmap(this.HelplineRulerCtrl1.Bmp);

            this.HelplineRulerCtrl1.AddDefaultHelplines();
            this.HelplineRulerCtrl1.ResetAllHelpLineLabelsColor();

            this.HelplineRulerCtrl1.PostPaint += Helplinerulerctrl1_Paint;

            this.CurPath = (GraphicsPath)curPath.Clone();
            this.CurPath2 = (GraphicsPath)curPath.Clone();

            this._r = this.CurPath.GetBounds();
            this._r2 = this.CurPath.GetBounds();
            this.Label6.Text = "Original Path: " + this._r.ToString();
            this.Label7.Text = "Transformed Path: " + this._r.ToString();
            this._dontDoUpdateNums = true;
            this.NumericUpDown6.Value = System.Convert.ToDecimal(this._r2.Width);
            this.NumericUpDown7.Value = System.Convert.ToDecimal(this._r2.Height);
            this._dontDoUpdateNums = false;
        }

        private void Helplinerulerctrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this.CurPath2 != null && this.CurPath2.PointCount > 1)
            {
                using (GraphicsPath gp = (GraphicsPath)this.CurPath2.Clone())
                {
                    float w = System.Convert.ToSingle(this.NumericUpDown1.Value);
                    int x = this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                    int y = this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                    using (Matrix m = new Matrix(this.HelplineRulerCtrl1.Zoom, 0, 0, this.HelplineRulerCtrl1.Zoom, x, y))
                    {
                        gp.Transform(m);
                        using (Pen pen = new Pen(new SolidBrush(this._highlightColor), w))
                        {
                            e.Graphics.DrawPath(pen, gp);
                        }
                    }
                }
            }
        }

        private void CheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox12.Checked)
                this.HelplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.HelplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            CheckBox12_CheckedChanged(this.CheckBox12, new EventArgs());
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null)
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            this.ColorDialog1.Color = this._highlightColor;
            if (this.ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                this._highlightColor = this.ColorDialog1.Color;
                this.Button13.BackColor = this._highlightColor;
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (CurPath != null && CurPath.PointCount > 0)
            {
                this._dontDoUpdateNums = true;
                this.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();
                float scaleX = System.Convert.ToSingle(this.NumericUpDown2.Value);
                float scaleY = System.Convert.ToSingle(this.NumericUpDown3.Value);
                float shiftX = System.Convert.ToSingle(this.NumericUpDown4.Value);
                float shiftY = System.Convert.ToSingle(this.NumericUpDown5.Value);
                using (GraphicsPath gp = (GraphicsPath)this.CurPath.Clone())
                {
                    RectangleF r = gp.GetBounds();
                    using (Matrix mx = new Matrix(1, 0, 0, 1, (float)(-r.X - (r.Width / (double)2.0F)), (float)(-r.Y - (r.Height / (double)2.0F))))
                    {
                        gp.Transform(mx);
                    }
                    using (Matrix mx = new Matrix(scaleX, 0, 0, scaleY, 0, 0))
                    {
                        gp.Transform(mx);
                    }
                    // Dim r2 As RectangleF = gp.GetBounds()
                    using (Matrix mx = new Matrix(1, 0, 0, 1, (float)(r.X + (r.Width / (double)2.0F) + shiftX), (float)(r.Y + (r.Height / (double)2.0F) + shiftY)))
                    {
                        gp.Transform(mx);
                    }

                    if (gp.PointCount > 1)
                    {
                        GraphicsPath? oCP2 = this.CurPath2;
                        this.CurPath2 = (GraphicsPath)gp.Clone();
                        this._r2 = this.CurPath2.GetBounds();
                        this.Label7.Text = "Transformed Path: " + this._r2.ToString();
                        if (oCP2 != null)
                        {
                            oCP2.Dispose();
                            oCP2 = null;
                        }
                        this.NumericUpDown6.Value = System.Convert.ToDecimal(this._r2.Width);
                        this.NumericUpDown7.Value = System.Convert.ToDecimal(this._r2.Height);
                        this.HelplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
                this.Enabled = true;
                this.Cursor = Cursors.Default;
            }

            this._dontDoUpdateNums = false;
        }

        private void NumericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (!this._dontDoUpdateNums)
            {
                float scaleX = (float)((double)this.NumericUpDown6.Value / (double)this._r.Width);
                float scaleY = (float)((double)this.NumericUpDown7.Value / (double)this._r.Height);

                this.NumericUpDown2.Value = (decimal)(scaleX);
                this.NumericUpDown3.Value = (decimal)(scaleY);
            }
        }
    }
}
