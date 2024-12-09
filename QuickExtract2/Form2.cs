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

namespace QuickExtract2
{
    public partial class Form2 : Form
    {
        private GraphicsPath? _fPath = null;

        public Form2(GraphicsPath fPath)
        {
            InitializeComponent();
            _fPath = fPath;

            RectangleF r = _fPath.GetBounds();

            Bitmap bmp = new Bitmap(System.Convert.ToInt32(Math.Ceiling(r.X + r.Width)) + 1, System.Convert.ToInt32(Math.Ceiling(r.Y + r.Height)) + 1);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillPath(Brushes.AliceBlue, _fPath);
                g.DrawPath(Pens.Black, _fPath);
            }

            if (this._fPath.PointCount > 0)
                this.Text = this._fPath.PathPoints.Length.ToString() + " PathPoints";

            this.pictureBox1.Image = bmp;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_fPath != null)
                _fPath.Dispose();

            if (this.pictureBox1.Image != null)
                this.pictureBox1.Image.Dispose();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox1.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                this.pictureBox1.Location = new Point(0, 0);
                this.pictureBox1.Dock = DockStyle.Fill;
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                this.pictureBox1.Location = new Point(0, 0);
                this.pictureBox1.Dock = DockStyle.None;
                this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.pictureBox1 != null)
            {
                if (keyData == Keys.Down)
                {
                    Panel1.AutoScrollPosition = new Point(-Panel1.AutoScrollPosition.X, -Panel1.AutoScrollPosition.Y + 4);
                    return true;
                }
                if (keyData == Keys.Up)
                {
                    Panel1.AutoScrollPosition = new Point(-Panel1.AutoScrollPosition.X, -Panel1.AutoScrollPosition.Y - 4);
                    return true;
                }
                if (keyData == Keys.Left)
                {
                    Panel1.AutoScrollPosition = new Point(-Panel1.AutoScrollPosition.X - 4, -Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.Right)
                {
                    Panel1.AutoScrollPosition = new Point(-Panel1.AutoScrollPosition.X + 4, -Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.PageDown)
                {
                    Panel1.AutoScrollPosition = new Point(-Panel1.AutoScrollPosition.X, -Panel1.AutoScrollPosition.Y + Panel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.PageUp)
                {
                    Panel1.AutoScrollPosition = new Point(-Panel1.AutoScrollPosition.X, -Panel1.AutoScrollPosition.Y - Panel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.Home)
                {
                    Panel1.AutoScrollPosition = new Point(-Int16.MaxValue, -Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.End)
                {
                    Panel1.AutoScrollPosition = new Point(Int16.MaxValue, -Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == (Keys.Home | Keys.Control))
                {
                    Panel1.AutoScrollPosition = new Point(-Int16.MaxValue, -Int16.MaxValue);
                    return true;
                }
                if (keyData == (Keys.End | Keys.Control))
                {
                    Panel1.AutoScrollPosition = new Point(Int16.MaxValue, Int16.MaxValue);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
