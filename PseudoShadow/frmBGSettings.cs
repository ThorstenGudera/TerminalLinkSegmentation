using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudoShadow
{
    public partial class frmBGSettings : Form
    {
        private bool _updating;
        public System.Drawing.Size BmpSize { get; set; }

        public frmBGSettings(Size sz, Size curSize)
        {
            InitializeComponent();

            this.BmpSize = sz;

            this.numWidth.Value = System.Convert.ToDecimal(this.BmpSize.Width);
            this.numHeight.Value = System.Convert.ToDecimal(this.BmpSize.Height);
            this.numFWidth.Value = 1M;
            this.numFHeight.Value = 1M;

            this.textBox1.Text = curSize.ToString();
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!_updating && this.Visible && this.BmpSize.Width > 0 && this.BmpSize.Height > 0)
            {
                _updating = true;
                if (System.Convert.ToInt32(this.numWidth.Value) != System.Convert.ToInt32(this.BmpSize.Width * System.Convert.ToDouble(this.numFWidth.Value)))
                {
                    this.numFWidth.Value = System.Convert.ToDecimal(System.Convert.ToDouble(this.numWidth.Value) / System.Convert.ToDouble(this.BmpSize.Width));

                    if (this.CheckBox2.Checked)
                        this.numHeight.Value = Math.Max(System.Convert.ToDecimal(this.BmpSize.Height * System.Convert.ToDouble(this.numFWidth.Value)), System.Convert.ToDecimal(1));
                }
            }
            _updating = false;
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!_updating && this.Visible && this.BmpSize.Width > 0 && this.BmpSize.Height > 0)
            {
                _updating = true;
                if (System.Convert.ToInt32(this.numHeight.Value) != System.Convert.ToInt32(this.BmpSize.Height * System.Convert.ToDouble(this.numFHeight.Value)))
                {
                    this.numFHeight.Value = System.Convert.ToDecimal(System.Convert.ToDouble(this.numHeight.Value) / System.Convert.ToDouble(this.BmpSize.Height));

                    if (this.CheckBox2.Checked)
                        this.numWidth.Value = Math.Max(System.Convert.ToDecimal(this.BmpSize.Width * System.Convert.ToDouble(this.numFHeight.Value)), System.Convert.ToDecimal(1));
                }
            }
            _updating = false;
        }

        private void numFWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!_updating && this.Visible && this.BmpSize.Width > 0 && this.BmpSize.Height > 0)
            {
                _updating = true;
                if (System.Convert.ToDouble(this.numFWidth.Value) != System.Convert.ToDouble(this.numWidth.Value) / System.Convert.ToDouble(this.BmpSize.Width))
                {
                    this.numWidth.Value = Math.Max(System.Convert.ToDecimal(this.BmpSize.Width * System.Convert.ToDouble(this.numFWidth.Value)), System.Convert.ToDecimal(1));

                    if (this.CheckBox2.Checked)
                        this.numFHeight.Value = System.Convert.ToDecimal(this.numFWidth.Value);
                }
            }
            _updating = false;
        }

        private void numFHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!_updating && this.Visible && this.BmpSize.Width > 0 && this.BmpSize.Height > 0)
            {
                _updating = true;
                if (System.Convert.ToDouble(this.numFHeight.Value) != System.Convert.ToDouble(this.numHeight.Value) / System.Convert.ToDouble(this.BmpSize.Height))
                {
                    this.numHeight.Value = Math.Max(System.Convert.ToDecimal(this.BmpSize.Height * System.Convert.ToDouble(this.numFHeight.Value)), System.Convert.ToDecimal(1));

                    if (this.CheckBox2.Checked)
                        this.numFWidth.Value = System.Convert.ToDecimal(this.numFHeight.Value);
                }
            }
            _updating = false;
        }

        private void frmBGSettings_Shown(object sender, EventArgs e)
        {
            if (this.BmpSize.Width > 0)
            {
                if (Math.Max(this.BmpSize.Width, this.BmpSize.Height) == this.BmpSize.Width)
                {
                    this.numWidth.Select();
                    this.numWidth.Select(0, this.numWidth.Value.ToString().Length);
                }
                else
                {
                    this.numHeight.Select();
                    this.numHeight.Select(0, this.numHeight.Value.ToString().Length);
                }
            }
        }
    }
}
