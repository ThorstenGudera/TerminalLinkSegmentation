using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmShiftPic : Form
    {
        private Bitmap? _bmpUpper = null;
        private bool _dontDoZoom;
        private bool _dontRedraw;
        private bool _tracking;
        private int _ix;
        private int _iy; 
        //private int _offsetX;
        //private int _offsetY;

        public frmShiftPic(Bitmap bmpLower, Bitmap bmpUpper)
        {
            InitializeComponent();

            if (AvailMem.AvailMem.checkAvailRam(bmpLower.Width * bmpLower.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = new Bitmap(bmpLower);
                this._bmpUpper = new Bitmap(bmpUpper);
            }

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
            this.helplineRulerCtrl1.dbPanel1.MouseMove += helplineRulerCtrl1_MouseMove;
            this.helplineRulerCtrl1.dbPanel1.MouseUp += helplineRulerCtrl1_MouseUp;

            this.helplineRulerCtrl1.PostPaint += helplineRulerCtrl1_Paint;

            this._dontDoZoom = true;
            this.cmbZoom.SelectedIndex = 4;
            this._dontDoZoom = false;
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                //this._offsetX = _ix - (int)this.numX.Value; 
                //this._offsetY = _iy - (int)this.numY.Value;

                this._tracking = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this._tracking)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                int oX = ix - _ix;
                int oY = iy - _iy;

                this._dontRedraw = true;
                this.numX.Value += oX;
                this.numY.Value += oY;
                this._dontRedraw = false;

                _ix = ix;
                _iy = iy;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if(this._tracking)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                int oX = ix - _ix;
                int oY = iy - _iy;

                this._dontRedraw = true;
                this.numX.Value += oX;
                this.numY.Value += oY;
                this._dontRedraw = false;
            }

            this._tracking = false;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (!this._tracking)
            {
                int x = (int)this.numX.Value;
                int y = (int)this.numY.Value;

                if (this._bmpUpper != null)
                    e.Graphics.DrawImage(this._bmpUpper, (float)(x * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                         (float)(y * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                         this._bmpUpper.Width * this.helplineRulerCtrl1.Zoom,
                                                         this._bmpUpper.Height * this.helplineRulerCtrl1.Zoom);
            }
        }

        private void frmShiftPic_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._bmpUpper != null)
                this._bmpUpper.Dispose();
        }

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(cmbZoom?.SelectedItem?.ToString());
                this.helplineRulerCtrl1.Enabled = true;
                if (this.cmbZoom?.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void frmShiftPic_Load(object sender, EventArgs e)
        {
            this.cmbZoom.Items.Add((0.75F).ToString());
            this.cmbZoom.Items.Add((0.5F).ToString());
            this.cmbZoom.Items.Add((0.25F).ToString());

            cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbZoom.SelectedIndex = 4;
        }

        private void numX_ValueChanged(object sender, EventArgs e)
        {
            if (!this._dontRedraw)
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void numY_ValueChanged(object sender, EventArgs e)
        {
            if (!this._dontRedraw)
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }
    }
}
