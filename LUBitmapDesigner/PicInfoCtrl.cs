﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LUBitmapDesigner
{
    public partial class PicInfoCtrl : UserControl
    {
        BitmapShape? _curShape = null;
        private bool _dontRaise;

        public event EventHandler<BitmapShape>? ShapeChanged;

        public PicInfoCtrl()
        {
            InitializeComponent();
        }

        public void SetValues(BitmapShape e)
        {
            if (e != null)
            {
                _curShape = e;

                RectangleF r = e.Bounds;
                float rot = e.Rotation;

                bool chckd = this.cbAspect.Checked;
                this.cbAspect.Checked = false;

                this.numX.Value = (decimal)r.X;
                this.numY.Value = (decimal)r.Y;
                this.numW.Value = (decimal)r.Width;
                this.numH.Value = (decimal)r.Height;
                this.numRot.Value = (decimal)rot;

                if( this.cmbMergeOP.SelectedIndex > -1)
                    this.cmbMergeOP.SelectedIndex = (int)this._curShape.MergeOperation;

                this.cbAspect.Checked = chckd;
            }
        }

        private void numX_ValueChanged(object sender, EventArgs e)
        {
            if (this._curShape != null)
            {
                if (this.cbIntVals.Checked)
                    this._curShape.Bounds = new Rectangle((int)this.numX.Value, (int)this._curShape.Bounds.Y, (int)this._curShape.Bounds.Width, (int)this._curShape.Bounds.Height);
                else
                    this._curShape.Bounds = new RectangleF((float)this.numX.Value, this._curShape.Bounds.Y, this._curShape.Bounds.Width, this._curShape.Bounds.Height);
                ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void numY_ValueChanged(object sender, EventArgs e)
        {
            if (this._curShape != null)
            {
                if (this.cbIntVals.Checked)
                    this._curShape.Bounds = new Rectangle((int)this._curShape.Bounds.X, (int)this.numY.Value, (int)this._curShape.Bounds.Width, (int)this._curShape.Bounds.Height);
                else
                    this._curShape.Bounds = new RectangleF(this._curShape.Bounds.X, (float)this.numY.Value, this._curShape.Bounds.Width, this._curShape.Bounds.Height);
                ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void numW_ValueChanged(object sender, EventArgs e)
        {
            if (this._curShape != null && this._curShape.Bmp != null)
            {
                if (this.cbAspect.Checked)
                {
                    double a = (double)this._curShape.Bmp.Width / (double)this._curShape.Bmp.Height;
                    double h = (double)this.numW.Value / a;

                    this._dontRaise = true;
                    if (this.cbIntVals.Checked)
                        this._curShape.Bounds = new Rectangle((int)this._curShape.Bounds.X, (int)this._curShape.Bounds.Y, (int)this.numW.Value, (int)this._curShape.Bounds.Height);
                    else
                        this._curShape.Bounds = new RectangleF(this._curShape.Bounds.X, this._curShape.Bounds.Y, (float)this.numW.Value, (float)h);
                    this._dontRaise = false;
                }
                else
                {
                    if (this.cbIntVals.Checked)
                        this._curShape.Bounds = new Rectangle((int)this._curShape.Bounds.X, (int)this._curShape.Bounds.Y, (int)this.numW.Value, (int)this._curShape.Bounds.Height);
                    else
                        this._curShape.Bounds = new RectangleF(this._curShape.Bounds.X, this._curShape.Bounds.Y, (float)this.numW.Value, this._curShape.Bounds.Height);
                }
                if (!this._dontRaise)
                    ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void numH_ValueChanged(object sender, EventArgs e)
        {
            if (this._curShape != null && this._curShape.Bmp != null)
            {
                if (this.cbAspect.Checked)
                {
                    double a = (double)this._curShape.Bmp.Width / (double)this._curShape.Bmp.Height;

                    double w = (double)this.numH.Value * a;

                    this._dontRaise = true;
                    if (this.cbIntVals.Checked)
                        this._curShape.Bounds = new Rectangle((int)this._curShape.Bounds.X, (int)this._curShape.Bounds.Y, (int)this._curShape.Bounds.Width, (int)this.numH.Value);
                    else
                        this._curShape.Bounds = new RectangleF(this._curShape.Bounds.X, this._curShape.Bounds.Y, (float)w, (float)this.numH.Value);
                    this._dontRaise = false;
                }
                else
                {
                    if (this.cbIntVals.Checked)
                        this._curShape.Bounds = new Rectangle((int)this._curShape.Bounds.X, (int)this._curShape.Bounds.Y, (int)this._curShape.Bounds.Width, (int)this.numH.Value);
                    else
                        this._curShape.Bounds = new RectangleF(this._curShape.Bounds.X, this._curShape.Bounds.Y, this._curShape.Bounds.Width, (float)this.numH.Value);
                }

                if (!this._dontRaise)
                    ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void btnOrigSize_Click(object sender, EventArgs e)
        {
            if (this._curShape != null && this._curShape.Bmp != null)
            {
                this._curShape.Bounds = new RectangleF(this._curShape.Bounds.X, this._curShape.Bounds.Y, this._curShape.Bmp.Width, this._curShape.Bmp.Height);

                if (!this._dontRaise)
                    ShapeChanged?.Invoke(this, this._curShape);

                this.SetValues(this._curShape);
            }
        }

        private void numRot_ValueChanged(object sender, EventArgs e)
        {
            if (this._curShape != null)
            {
                this._curShape.Rotation = (float)this.numRot.Value;

                if (!this._dontRaise)
                    ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            if (this._curShape != null)
            {
                this._curShape.Opacity = (float)this.numOpacity.Value;

                if (!this._dontRaise)
                    ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void cbLock_CheckedChanged(object sender, EventArgs e)
        {
            this.SetControls(!this.cbLock.Checked);
            this.Refresh();
        }

        private void SetControls(bool chckd)
        {
            if (this._curShape != null)
            {
                foreach (Control c in this.GroupBox1.Controls)
                    if (!c.Equals(this.cbLock))
                        c.Enabled = chckd;

                if (!this._dontRaise)
                    ShapeChanged?.Invoke(this, this._curShape);
            }
        }

        private void cbIntVals_CheckedChanged(object sender, EventArgs e)
        {
            if (this._curShape != null && this.cbIntVals.Checked)
                this.SetIntVals();
        }

        private bool CheckDrawInt()
        {
            if (this._curShape != null)
            {
                return ((this._curShape.Bounds.X * this._curShape.Zoom - (int)this._curShape.Bounds.X * this._curShape.Zoom == 0) &&
                     (this._curShape.Bounds.Y * this._curShape.Zoom - (int)this._curShape.Bounds.Y * this._curShape.Zoom == 0) &&
                     (this._curShape.Bounds.Width * this._curShape.Zoom - (int)this._curShape.Bounds.Width * this._curShape.Zoom == 0) &&
                     (this._curShape.Bounds.Height * this._curShape.Zoom - (int)this._curShape.Bounds.Height * this._curShape.Zoom == 0));
            }

            return false;
        }

        private void SetIntVals()
        {
            if (this._curShape != null)
            {
                this._dontRaise = true;
                this.numX.Value = (int)this.numX.Value;
                this.numY.Value = (int)this.numY.Value;
                this.numW.Value = (int)this.numW.Value;
                this._dontRaise = false;
                this.numH.Value = (int)this.numH.Value;
            }
        }

        private void cmbMergeOP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._curShape != null)
            {
                this._curShape.MergeOperation = (MergeOperation)this.cmbMergeOP.SelectedIndex;
                ShapeChanged?.Invoke(this, this._curShape);
            }
        }
    }
}
