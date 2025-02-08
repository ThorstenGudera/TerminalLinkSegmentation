using ChainCodeFinder;
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
        private List<Tuple<int, int, int, bool, List<List<Point>>>>? _scribbleSeq;
        private float _zoom;
        private Rectangle? _rc;
        private List<PointF>? _displayPoints;
        private List<Point>? _currentList;
        private bool _dynamic = true;
        private int _divisor = 2;
        private int _newWidth = 10;

        public frmLastScribbles(Bitmap bmp, Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles, List<Tuple<int, int, int, bool, List<List<Point>>>>? scribbleSeq)
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
                this.cbApproxLines.Enabled = this.btnSmothenSettings.Enabled = this.btnApproxLines.Enabled = false;

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

                if (fg == 3 && this.listBox1.Items.Count > 0)
                    this.cbApproxLines.Enabled = this.btnSmothenSettings.Enabled = this.btnApproxLines.Enabled = true;
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

                int curW = -1;

                if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out curW))
                    this.numNewWidth.Value = (decimal)curW;
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
                                    IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                                    if (l != null && l.Count() > 0)
                                    {
                                        IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> whL = l.Where(a => a.Item2 == wh);

                                        if (whL != null && whL.Count() > 0)
                                        {
                                            IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> listL = whL.Where(a => a.Item3 == this.listBox2.SelectedIndex);

                                            if (listL != null && listL.Count() > 0)
                                            {
                                                int indxt = this._scribbleSeq.IndexOf(listL.First());

                                                for (int j4 = indxt + 1; j4 < this._scribbleSeq.Count; j4++)
                                                {
                                                    if (this._scribbleSeq[j4].Item1 == fg && this._scribbleSeq[j4].Item2 == wh)
                                                        this._scribbleSeq[j4] = Tuple.Create(fg, wh, this._scribbleSeq[j4].Item3 - 1, false, new List<List<Point>>());
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

        private void btnSetWidth_Click(object sender, EventArgs e)
        {
            //while testing...
            try
            {
                if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
                {
                    int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;

                    if (this._scribbles.ContainsKey(fg))
                    {
                        Dictionary<int, List<List<Point>>> z = this._scribbles[fg];
                        int newW = (int)this.numNewWidth.Value;

                        if (this.rbAll.Checked)
                        {
                            if (z.Keys.Count > 0)
                            {
                                if (!z.ContainsKey(newW))
                                    z.Add(newW, new List<List<Point>>());
                                if (this._scribbleSeq == null)
                                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                                int curW = -1;

                                if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out curW))
                                {
                                    if (newW != curW)
                                    {
                                        foreach (List<Point> j in z[curW])
                                        {
                                            List<Point> jj = new List<Point>();
                                            jj.AddRange(j.ToArray());
                                            z[newW].Add(jj);

                                            DisplayPointsInPic(jj);
                                        }

                                        for (int i = z.Count - 1; i >= 0; i--)
                                            z.Remove(curW);

                                        if (this._scribbles[fg].ContainsKey(newW))
                                            if (this._scribbles[fg][newW].Count > 0)
                                            {
                                                if (this._scribbleSeq != null)
                                                {
                                                    IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                                                    if (l != null && l.Count() > 0)
                                                    {
                                                        List<Tuple<int, int, int, bool, List<List<Point>>>> whL = l.Where(a => a.Item2 == curW).ToList();

                                                        if (whL != null && whL.Count() > 0)
                                                        {
                                                            List<int> lll = new List<int>();
                                                            foreach (var llll in whL)
                                                                lll.Add(llll.Item3);

                                                            for (int i = lll.Count - 1; i >= 0; i--)
                                                            {
                                                                IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> listL = whL.Where(a => a.Item3 == lll[i]);

                                                                if (listL != null && listL.Count() > 0)
                                                                {
                                                                    List<Tuple<int, int, int, bool, List<List<Point>>>> listL2 = listL.ToList();
                                                                    for (int jjj = 0; jjj < listL2.Count; jjj++)
                                                                    {
                                                                        int indx = this._scribbleSeq.IndexOf(listL2[jjj]);
                                                                        this._scribbleSeq[indx] = Tuple.Create(fg, newW, listL2[jjj].Item3, listL2[jjj].Item4, listL2[jjj].Item5);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                    }
                                }
                            }
                        }
                        else if (this.rbOne.Checked)
                        {
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

                                            List<Point> nL = new List<Point>();
                                            nL.AddRange(this._scribbles[fg][wh][this.listBox2.SelectedIndex]);

                                            if (!this._scribbles[fg].ContainsKey(newW))
                                                this._scribbles[fg].Add(newW, new List<List<Point>>());

                                            this._scribbles[fg][newW].Add(nL);
                                            this._scribbles[fg][wh].RemoveAt(this.listBox2.SelectedIndex);

                                            DisplayPointsInPic(nL);

                                            if (this._scribbleSeq != null)
                                            {
                                                IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                                                if (l != null && l.Count() > 0)
                                                {
                                                    IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> whL = l.Where(a => a.Item2 == wh);

                                                    if (whL != null && whL.Count() > 0)
                                                    {
                                                        IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> listL = whL.Where(a => a.Item3 == this.listBox2.SelectedIndex);

                                                        if (listL != null && listL.Count() > 0)
                                                        {
                                                            int indxt = this._scribbleSeq.IndexOf(listL.First());

                                                            for (int j4 = indxt + 1; j4 < this._scribbleSeq.Count; j4++)
                                                            {
                                                                if (this._scribbleSeq[j4].Item1 == fg && this._scribbleSeq[j4].Item2 == wh)
                                                                    this._scribbleSeq[j4] = Tuple.Create(fg, wh, this._scribbleSeq[j4].Item3 - 1, false, new List<List<Point>>());
                                                            }

                                                            Tuple<int, int, int, bool, List<List<Point>>> oldTp = listL.First();

                                                            this._scribbleSeq.Remove(listL.First());

                                                            IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> whN = l.Where(a => a.Item2 == (int)this.numNewWidth.Value);

                                                            if (whN == null || whN.Count() == 0)
                                                            {
                                                                Tuple<int, int, int, bool, List<List<Point>>> newTp = Tuple.Create(oldTp.Item1, (int)this.numNewWidth.Value,
                                                                   0, oldTp.Item4, oldTp.Item5);

                                                                this._scribbleSeq.Add(newTp);
                                                            }
                                                            else
                                                            {
                                                                Tuple<int, int, int, bool, List<List<Point>>> newTp = Tuple.Create(oldTp.Item1, (int)this.numNewWidth.Value,
                                                                    whN.Count(), oldTp.Item4, oldTp.Item5);

                                                                this._scribbleSeq.Add(newTp);
                                                            }
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

                    if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
                    {
                        fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
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
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString() + "\n\nBetter reload this form.");
            }
        }

        private void btnSetPos_Click(object sender, EventArgs e)
        {
            //while testing...
            try
            {
                if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
                {
                    int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;

                    if (this._scribbles.ContainsKey(fg))
                    {
                        Dictionary<int, List<List<Point>>> z = this._scribbles[fg];
                        int x = (int)this.numTX.Value;
                        int y = (int)this.numTY.Value;

                        int curW = -1;

                        if (this.rbTAll.Checked)
                        {
                            if (z.Keys.Count > 0)
                            {
                                if (this._scribbleSeq == null)
                                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                                if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out curW))
                                {
                                    if (this._scribbles[fg].ContainsKey(curW))
                                        if (this._scribbles[fg][curW].Count > 0)
                                        {
                                            for (int j = 0; j < this._scribbles[fg][curW].Count; j++)
                                            {
                                                List<Point>? l = this._scribbles[fg][curW][j];

                                                for (int jj = 0; jj < l.Count; jj++)
                                                    l[jj] = new Point(l[jj].X + x, l[jj].Y + y);

                                                DisplayPointsInPic(l);
                                            }

                                            if (this._scribbleSeq != null)
                                            {
                                                IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                                                if (l != null && l.Count() > 0)
                                                {
                                                    List<Tuple<int, int, int, bool, List<List<Point>>>> whL = l.Where(a => a.Item2 == curW).ToList();

                                                    if (whL != null && whL.Count() > 0)
                                                    {
                                                        for (int j = 0; j < whL.Count; j++)
                                                        {
                                                            List<List<Point>>? readL = whL[j].Item5;

                                                            for (int jj = 0; jj < readL.Count; jj++)
                                                            {
                                                                List<Point>? pts = new List<Point>();
                                                                List<Point>? ptsOld = readL[jj];

                                                                for (int i = 0; i < ptsOld.Count; i++)
                                                                    pts.Add(new Point(ptsOld[i].X + x, ptsOld[i].Y + y));

                                                                readL[jj] = pts;
                                                            }

                                                            whL[j] = Tuple.Create(whL[j].Item1, whL[j].Item2, whL[j].Item3, whL[j].Item4, readL);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                }
                            }
                        }
                        else if (this.rbTOne.Checked)
                        {
                            if (z.Keys.Count > 0)
                            {
                                if (this._scribbleSeq == null)
                                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                                if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out curW))
                                {
                                    if (this._scribbles[fg].ContainsKey(curW))
                                        if (this._scribbles[fg][curW].Count > 0)
                                        {
                                            int curL = this.listBox2.SelectedIndex;

                                            if (this.listBox2.SelectedIndex > -1)
                                            {
                                                List<Point>? l = this._scribbles[fg][curW][curL];
                                                if (l != null)
                                                {
                                                    for (int jj = 0; jj < l.Count; jj++)
                                                        l[jj] = new Point(l[jj].X + x, l[jj].Y + y);

                                                    this._scribbles[fg][curW][curL] = l;

                                                    DisplayPointsInPic(l);
                                                }
                                            }

                                            if (this._scribbleSeq != null)
                                            {
                                                IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                                                if (l != null && l.Count() > 0)
                                                {
                                                    List<Tuple<int, int, int, bool, List<List<Point>>>> whL = l.Where(a => a.Item2 == curW).ToList();

                                                    if (whL != null && whL.Count() > 0)
                                                    {
                                                        IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> listL = whL.Where(a => a.Item3 == curL);
                                                        if (listL != null && listL.Count() > 0)
                                                        {
                                                            Tuple<int, int, int, bool, List<List<Point>>> jj = listL.First();
                                                            List<List<Point>>? readA = jj.Item5;

                                                            if (readA != null && readA.Count > 0)
                                                            {
                                                                for (int i = 0; i < readA.Count; i++)
                                                                {
                                                                    List<Point>? readL = readA[i];

                                                                    if (readL != null && readL.Count > 0)
                                                                    {
                                                                        List<Point>? pts = new List<Point>();
                                                                        for (int ii = 0; ii < readL.Count; ii++)
                                                                            pts.Add(new Point(readL[ii].X + x, readL[ii].Y + y));

                                                                        readA[i] = pts;
                                                                    }

                                                                    jj = Tuple.Create(jj.Item1, jj.Item2, jj.Item3, jj.Item4, readA);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString() + "\n\nBetter reload this form.");
            }
        }

        private void btnSmothenSettings_Click(object sender, EventArgs e)
        {
            using frmLoadScribblesSettings frm = new frmLoadScribblesSettings();

            frm.rbDynamic.Checked = this._dynamic;
            frm.numDivisor.Value = (decimal)this._divisor;
            frm.numNewWidth.Value = (decimal)this._newWidth;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                this._dynamic = frm.rbDynamic.Checked;
                this._divisor = (int)frm.numDivisor.Value;
                this._newWidth = (int)frm.numNewWidth.Value;
            }
        }

        private void btnApproxLines_Click(object sender, EventArgs e)
        {
            //test, this might help for scribbles that come from frmQuickExtract
            if (this._scribbles != null && this.cbApproxLines.Checked && this._scribbles.ContainsKey(3) && this.listBox2.SelectedIndex > -1)
            {
                Dictionary<int, List<List<Point>>> j = this._scribbles[3];

                List<List<Point>> l = j.ElementAt(this.listBox2.SelectedIndex).Value;
                int w = j.ElementAt(this.listBox2.SelectedIndex).Key;
                ChainFinder cf = new ChainFinder();

                for (int ii = 0; ii < l.Count; ii++)
                {
                    List<Point> pts = l[ii];
                    if (this._dynamic)
                        pts = cf.ApproximateLines(pts, Math.Max(w / this._divisor, 1));
                    else
                        pts = cf.ApproximateLines(pts, this._newWidth);

                    List<Point> pts2 = new List<Point>();
                    for (int ll = 1; ll < pts.Count; ll++)
                    {
                        double dx = pts[ll].X - pts[ll - 1].X;
                        double dy = pts[ll].Y - pts[ll - 1].Y;
                        double lngth = Math.Sqrt(dx * dx + dy * dy);
                        dx /= lngth;
                        dy /= lngth;

                        for (int iii = 0; iii < (int)lngth; iii++)
                        {
                            Point pt = new Point(pts[ll - 1].X + (int)(iii * dx), pts[ll - 1].Y + (int)(iii * dy));

                            if (pts[ll - 1].X != pt.X || pts[ll - 1].Y != pt.Y)
                                pts2.Add(pt);
                        }
                    }

                    l[ii] = pts2;
                    j[w] = l;
                    this._scribbles[3] = j;

                    for (int ll = 1; ll < pts.Count; ll++)
                        DisplayPointsInPic(this._scribbles[3][w][ii]);
                }
            }
        }
    }
}
