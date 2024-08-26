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
    public partial class frmLastScribbles : Form
    {
        private Dictionary<int, Dictionary<int, List<List<Point>>>>? _scribbles;
        private List<Tuple<int, int, int>>? _scribbleSeq;
        private float _zoom;
        private Rectangle? _rc;
        private List<PointF>? _displayPoints;
        private List<Point>? _currentList;

        public frmLastScribbles(Bitmap bmp, Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles, List<Tuple<int, int, int>>? scribbleSeq)
        {
            InitializeComponent();
            this._scribbles = scribbles;
            this._scribbleSeq = scribbleSeq;
            this.pictureBox1.Image = bmp;

            this._rc = this.GetImageRectangle();
        }

        private void frmLastScribbles_Load(object sender, EventArgs e)
        {
            foreach (string z in System.Enum.GetNames(typeof(ScribblesType)))
                this.cmbScribblesType.Items.Add(z.ToString());

            this.cmbScribblesType.SelectedIndex = 0;
        }

        private void cnbScribblesType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                this.listBox1.Items.Clear();
                this.listBox2.Items.Clear();

                if (this._scribbles.ContainsKey(fg))
                {
                    Dictionary<int, List<List<Point>>> z = this._scribbles[fg];
                    if (z.Keys.Count > 0)
                    {
                        foreach (int i in z.Keys)
                        {
                            this.listBox1.Items.Add(i.ToString());
                        }
                    }
                }

                if (this.listBox1.Items.Count > 0)
                    this.listBox1.SelectedIndex = 0;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._scribbles != null)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                if (this._scribbles.ContainsKey(fg))
                {
                    int j = -1;

                    if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out j))
                    {
                        int wh = j;

                        List<List<Point>> list = this._scribbles[fg][wh];
                        int j2 = -1;

                        if (Int32.TryParse(this.listBox2.SelectedItem?.ToString(), out j2))
                            if (list.Count > this.listBox2.SelectedIndex && list[this.listBox2.SelectedIndex].Count == j2)
                            {
                                this._currentList = list[this.listBox2.SelectedIndex];
                                DisplayPointsInPic(_currentList);
                            }
                    }
                }
            }
        }

        private void DisplayPointsInPic(List<Point> points)
        {
            List<PointF> list = points.Select(a => new PointF(a.X * _zoom, a.Y * _zoom)).ToList();
            this._displayPoints = list;
            this.pictureBox1.Invalidate();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                if (this._scribbles.ContainsKey(fg))
                {
                    Dictionary<int, List<List<Point>>> z = this._scribbles[fg];
                    if (z.Keys.Count > 0)
                    {
                        this.listBox2.Items.Clear();

                        int j = -1;

                        if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out j))
                        {
                            List<List<Point>> wh = z[j];
                            for (int i = 0; i < wh.Count; i++)
                                this.listBox2.Items.Add(wh[i].Count.ToString());
                        }
                    }
                }

                if (this.listBox2.Items.Count > 0)
                    this.listBox2.SelectedIndex = 0;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this._scribbles != null && MessageBox.Show("Delete this item from the scribbles-collection?", "",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                if (this._scribbles.ContainsKey(fg))
                {
                    int j = -1;

                    if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out j))
                    {
                        int wh = j;

                        List<List<Point>> list = this._scribbles[fg][wh];
                        int j2 = -1;

                        if (Int32.TryParse(this.listBox2.SelectedItem?.ToString(), out j2))
                            if (list.Count > this.listBox2.SelectedIndex && list[this.listBox2.SelectedIndex].Count == j2)
                            {
                                this.listBox2.SuspendLayout();
                                this._scribbles[fg][wh].RemoveAt(this.listBox2.SelectedIndex);
                                if (this._scribbleSeq != null)
                                {
                                    IEnumerable<Tuple<int, int, int>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                                    if (l != null && l.Count() > 0)
                                    {
                                        IEnumerable<Tuple<int, int, int>> whL = l.Where(a => a.Item2 == wh);

                                        if (whL != null && whL.Count() > 0)
                                        {
                                            IEnumerable<Tuple<int, int, int>> listL = whL.Where(a => a.Item3 == this.listBox2.SelectedIndex);

                                            if (listL != null && listL.Count() > 0)
                                            {
                                                int indxt = this._scribbleSeq.IndexOf(listL.First());

                                                for (int j4 = indxt + 1; j4 < this._scribbleSeq.Count; j4++)
                                                {
                                                    if (this._scribbleSeq[j4].Item1 == fg && this._scribbleSeq[j4].Item2 == wh)
                                                        this._scribbleSeq[j4] = Tuple.Create(fg, wh, this._scribbleSeq[j4].Item3 - 1);
                                                }

                                                this._scribbleSeq.Remove(listL.First());
                                            }
                                        }
                                    }
                                }

                                listBox1_SelectedIndexChanged(listBox1, new EventArgs());
                                this.listBox2.ResumeLayout();
                            }
                    }
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            {
                this.pictureBox1.Dock = DockStyle.None;
                this.pictureBox1.Location = new Point(0, 0);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBox1.Dock = DockStyle.Fill;
            }

            this._rc = this.GetImageRectangle();
        }

        // get position of pic in picturebox
        private Rectangle? GetImageRectangle()
        {
            Type pboxType = this.pictureBox1.GetType();

            if (pboxType != null)
            {
                System.Reflection.PropertyInfo? irProperty = pboxType.GetProperty("ImageRectangle", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                Rectangle? r = null;

                if (irProperty != null)
                    r = (Rectangle?)irProperty.GetValue(this.pictureBox1, null);

                _zoom = System.Convert.ToSingle(r?.Width / (double)this.pictureBox1.Image.Width);

                if (r != null)
                    return r.Value;
            }

            return null;
        }

        private void frmLastScribbles_Layout(object sender, LayoutEventArgs e)
        {
            if (!this.pictureBox1.IsDisposed && this.WindowState != FormWindowState.Minimized && this.pictureBox1.Image != null)
            {
                this._rc = this.GetImageRectangle();
                if (this._currentList != null)
                    DisplayPointsInPic(_currentList);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (this._displayPoints != null)
            {
                foreach (PointF pt in this._displayPoints)
                {
                    if (this._rc != null)
                    {
                        float xx = this._rc.Value.X;
                        float yy = this._rc.Value.Y;
                        e.Graphics.FillRectangle(Brushes.Yellow, new RectangleF(pt.X + xx - 1, pt.Y + yy - 1, 3, 3));
                    }
                }
            }
        }
    }
}
