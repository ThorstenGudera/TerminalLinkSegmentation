using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http.Json;
using ChainCodeFinder;

namespace QuickExtract2
{
    public partial class frmSavedPaths : Form
    {
        public frmSavedPaths()
        {
            InitializeComponent();
        }

        private List<List<PointF>>? _selectedPath;
        private Color _highlightColor = Color.Lime;
        private List<int>? _checkedPaths;
        private Color _cpColor = Color.Red;
        public List<List<List<PointF>>>? PathList { get; set; }
        public List<List<PointF>>? CurPath { get; set; }

        private object _lockObject = new object();
        private frmQuickExtract? _frmQE;

        public event EventHandler<string>? BoundaryError;

        public frmSavedPaths(Bitmap bmp, List<List<PointF>> curPath, List<List<List<PointF>>>? pathList, frmQuickExtract frm)
        {
            InitializeComponent();
            this._frmQE = frm;

            this.helplineRulerCtrl1.Bmp = (Bitmap)bmp.Clone();

            // Me.HelplineRulerCtrl1.SetZoomOnlyByMethodCall = True

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

            this.helplineRulerCtrl1.PostPaint += Helplinerulerctrl1_Paint;

            if (pathList != null && pathList.Count > 0)
            {
                this.PathList = pathList;
                this.ListBox1.SuspendLayout();
                this.ListBox1.Items.Clear();
                for (int i = 0; i <= pathList.Count - 1; i++)
                    this.ListBox1.Items.Add("savedPath_" + i.ToString());
                this.ListBox1.ResumeLayout();
                if (this.ListBox1.Items.Count > 0)
                    this.ListBox1.SelectedIndex = 0;
            }

            this.CurPath = curPath;
        }

        private void Helplinerulerctrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this._checkedPaths == null)
            {
                if (this.CheckBox2.Checked)
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (this.CurPath?.Count > 0)
                        {
                            for (int i = 0; i <= this.CurPath.Count - 1; i++)
                            {
                                List<PointF> p = this.CurPath[i];
                                if (p != null && p.Count > 1)
                                    gp.AddLines(p.ToArray());
                            }
                        }

                        float w = System.Convert.ToSingle(this.NumericUpDown1.Value);
                        int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                        int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                        using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                        {
                            gp.Transform(m);
                            using (Pen pen = new Pen(new SolidBrush(this._cpColor), w))
                            {
                                e.Graphics.DrawPath(pen, gp);
                            }
                        }
                    }
                }
                if (this._selectedPath != null)
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (this._selectedPath.Count > 0)
                        {
                            for (int i = 0; i <= this._selectedPath.Count - 1; i++)
                            {
                                List<PointF> p = this._selectedPath[i];
                                if (p != null && p.Count > 1)
                                    gp.AddLines(p.ToArray());
                            }
                        }

                        float w = System.Convert.ToSingle(this.NumericUpDown1.Value);
                        int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                        int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                        using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
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

            if (this._checkedPaths != null && this._checkedPaths.Count > 0)
            {
                for (int j = 0; j <= this._checkedPaths.Count - 1; j++)
                {
                    if (this._checkedPaths[j] == -1)
                    {
                        // curpath
                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            if (this.CurPath?.Count > 0)
                            {
                                for (int i = 0; i <= this.CurPath.Count - 1; i++)
                                {
                                    List<PointF> p = this.CurPath[i];
                                    if (p != null && p.Count > 1)
                                        gp.AddLines(p.ToArray());
                                }
                            }

                            float w = System.Convert.ToSingle(this.NumericUpDown1.Value);
                            int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                            int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                            using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                            {
                                gp.Transform(m);
                                using (Pen pen = new Pen(new SolidBrush(this._highlightColor), w))
                                {
                                    e.Graphics.DrawPath(pen, gp);
                                }
                            }
                        }
                    }
                    else if (this.PathList != null)
                    {
                        List<List<PointF>> cPath = this.PathList[this._checkedPaths[j]];

                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            if (cPath.Count > 0)
                            {
                                for (int i = 0; i <= cPath.Count - 1; i++)
                                {
                                    List<PointF> p = cPath[i];
                                    if (p != null && p.Count > 1)
                                        gp.AddLines(p.ToArray());
                                }
                            }

                            float w = System.Convert.ToSingle(this.NumericUpDown1.Value);
                            int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                            int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                            using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
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
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1 && this.PathList != null)
            {
                this._selectedPath = this.PathList[this.ListBox1.SelectedIndex];
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
            else
            {
                this._selectedPath = null;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            this.ColorDialog1.Color = this.CheckBox4.Checked ? this._cpColor : this._highlightColor;
            if (this.ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.CheckBox4.Checked)
                    this._cpColor = this.ColorDialog1.Color;
                else
                    this._highlightColor = this.ColorDialog1.Color;

                this.Button13.BackColor = this._highlightColor;
                this.CheckBox4.BackColor = this._cpColor;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1 && this.PathList != null)
            {
                List<List<PointF>>? tmp = this.CurPath;
                if (tmp != null)
                {
                    this.CurPath = this.PathList[this.ListBox1.SelectedIndex];
                    this.PathList[this.ListBox1.SelectedIndex] = tmp;
                    ListBox1_SelectedIndexChanged(this.ListBox1, new EventArgs());
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    if (this.CheckBox3.Checked)
                    {
                        List<List<PointF>> fList = ClonePath(this.CurPath, 0);
                        this.PathList.Add(fList);
                        int i = this.ListBox1.Items.Count;
                        this.ListBox1.Items.Add("savedPath_" + i.ToString());
                    }

                    this.CheckBox2.Checked = true;
                }
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.Label4.Enabled = this.CheckBox2.Checked;
            this.Button8.Enabled = this.CheckBox2.Checked;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1 && MessageBox.Show("delete this path?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int j = this.ListBox1.SelectedIndex;
                this.PathList?.RemoveAt(j);
                this.ListBox1.SuspendLayout();
                this.ListBox1.Items.Clear();
                for (int i = 0; i <= PathList?.Count - 1; i++)
                    this.ListBox1.Items.Add("savedPath_" + i.ToString());
                this.ListBox1.ResumeLayout();
                if (this.ListBox1.Items.Count > 0)
                    this.ListBox1.SelectedIndex = j < this.ListBox1.Items.Count ? j : j - 1;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1 && this.PathList != null)
            {
                float addX = 0F;

                if (this.CheckBox1.Checked)
                    addX = 50;

                List<List<PointF>> p = this.PathList[this.ListBox1.SelectedIndex];
                this.PathList.Add(ClonePath(p, addX));
                this.ListBox1.Items.Add("savedPath_" + this.ListBox1.Items.Count.ToString());
                this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;
            }
        }

        private List<List<PointF>> ClonePath(List<List<PointF>> p, float addX)
        {
            List<List<PointF>> pathCopy = new List<List<PointF>>();

            for (int i = 0; i <= p.Count - 1; i++)
            {
                List<PointF> pts = new List<PointF>();
                List<PointF> q = p[i];

                if (q != null && q.Count > 1)
                {
                    for (int j = 0; j <= q.Count - 1; j++)
                        pts.Add(new PointF(q[j].X + addX, q[j].Y));
                    pathCopy.Add(pts);
                }
            }

            return pathCopy;
        }

        private void CheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox12.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            CheckBox12_CheckedChanged(this.CheckBox12, new EventArgs());
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if ((this.CurPath != null && this.CurPath.Count > 0) || (this.PathList != null && this.PathList.Count > 0))
            {
                GraphicsPath? gPath = null;

                if (this.PathList != null)
                    using (frmSelectCrop frm7 = new frmSelectCrop(this.PathList))
                    {
                        frm7.Label1.Text = "Select Paths to save:";
                        frm7.Label2.Enabled = false;
                        frm7.RadioButton1.Enabled = false;
                        frm7.RadioButton2.Enabled = false;
                        frm7.CheckBox1.Checked = false;

                        frm7.PathsChanged += frm7_PathsChanged;
                        if (frm7.ShowDialog() == DialogResult.OK)
                        {
                            CheckedListBox.CheckedIndexCollection fL = frm7.CheckedListBox1.CheckedIndices;
                            if (frm7.CheckBox1.Checked)
                            {
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    if (this.CurPath?.Count > 0)
                                    {
                                        for (int i = 0; i <= this.CurPath.Count - 1; i++)
                                        {
                                            List<PointF> p = this.CurPath[i];
                                            if (p != null && p.Count > 1)
                                                gp.AddLines(p.ToArray());
                                        }
                                        if (gp.PointCount > 1)
                                        {
                                            gPath = new GraphicsPath();
                                            gPath.AddPath((GraphicsPath)gp.Clone(), false);
                                        }
                                    }
                                }
                            }
                            if (fL.Count > 0)
                            {
                                for (int j = 0; j <= fL.Count - 1; j++)
                                {
                                    List<List<PointF>> path = this.PathList[fL[j]];

                                    if (path.Count > 0)
                                    {
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            for (int i = 0; i <= path.Count - 1; i++)
                                            {
                                                List<PointF> p = path[i];

                                                if (p != null && p.Count > 1)
                                                    gp.AddLines(p.ToArray());
                                            }

                                            if (gp.PointCount > 1)
                                            {
                                                if (gp.PathPoints[0].X == gp.PathPoints[gp.PathPoints.Length - 1].X && gp.PathPoints[0].Y == gp.PathPoints[gp.PathPoints.Length - 1].Y)
                                                    gp.CloseFigure();

                                                if (gPath == null)
                                                    gPath = new GraphicsPath();

                                                gPath.AddPath((GraphicsPath)gp.Clone(), false);
                                                gPath.CloseFigure();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        frm7.PathsChanged -= frm7_PathsChanged;
                        this._checkedPaths = null;
                    }

                if (gPath != null && gPath.PointCount > 1)
                {
                    if (this._frmQE != null && this._frmQE.PreResampleFactor != 1)
                    {
                        using Matrix mx = new Matrix(this._frmQE.PreResampleFactor, 0, 0, this._frmQE.PreResampleFactor, 0, 0);
                        gPath.Transform(mx);
                    }

                    if (this.SaveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            JsonSerializerOptions options = new()
                            {
                                NumberHandling =
                                    JsonNumberHandling.AllowReadingFromString |
                                    JsonNumberHandling.WriteAsString,
                                WriteIndented = true,
                                ReadCommentHandling = JsonCommentHandling.Skip,
                                AllowTrailingCommas = true,
                            };

                            SavedPath sp = new SavedPath();
                            sp.PathPoints = gPath.PathPoints;
                            sp.PathTypes = gPath.PathTypes;

                            string FileName = this.SaveFileDialog2.FileName;

                            using FileStream createStream = File.Create(FileName);
                            JsonSerializer.Serialize(createStream, sp, options);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                        }
                    }
                }

                if (gPath != null)
                    gPath.Dispose();
                gPath = null;
            }
        }

        private void frm7_PathsChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                frmSelectCrop f = (frmSelectCrop)sender;
                this._checkedPaths = new List<int>();

                if (f.CheckBox1.Checked)
                    this._checkedPaths.Add(-1);
                if (f.CheckedListBox1.CheckedIndices.Count > 0)
                {
                    for (int i = 0; i <= f.CheckedListBox1.CheckedIndices.Count - 1; i++)
                        this._checkedPaths.Add(f.CheckedListBox1.CheckedIndices[i]);
                }
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (this.OpenFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = this.OpenFileDialog2.FileName;
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;
                this.Refresh();
                SavedPath? sp = null;
                Tuple<PointF[], Byte[]>? tp = null;
                bool bError = false;
                Stream? stream = null;

                try
                {
                    JsonSerializerOptions options = new()
                    {
                        NumberHandling =
                        JsonNumberHandling.AllowReadingFromString |
                        JsonNumberHandling.WriteAsString,
                        WriteIndented = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                    };

                    stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    sp = JsonSerializer.Deserialize<SavedPath>(stream, options);

                    if (sp?.PathPoints == null)
                    {
                        stream.Position = 0;
                        tp = JsonSerializer.Deserialize<Tuple<PointF[], Byte[]>>(stream, options);
                    }
                }
                catch (Exception ex)
                {
                    bError = true;
                    Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                }

                try
                {
                    //stream.Close()
                    stream?.Dispose();
                    stream = null;
                }
                catch
                { }

                if (sp?.PathPoints != null)
                {
                    try
                    {
                        PointF[]? pts2 = sp.PathPoints; //(PointF[])o[0];
                        byte[]? tps = sp.PathTypes; //(byte[])o[1];

                        if (pts2 != null && tps != null)
                        {
                            GraphicsPath? gp = new GraphicsPath(pts2, tps);

                            if (this._frmQE != null && this._frmQE.PreResampleFactor != 1)
                            {
                                using Matrix mx = new Matrix(1.0f / this._frmQE.PreResampleFactor, 0, 0,
                                    1.0f / this._frmQE.PreResampleFactor, 0, 0);
                                gp.Transform(mx);
                            }

                            if (this.PathList == null)
                                this.PathList = new List<List<List<PointF>>>();

                            if (gp.PointCount > 1)
                            {
                                byte[] types = gp.PathTypes;
                                for (int i = 0; i <= types.Length - 1; i++)
                                {
                                    int j = i;
                                    List<List<PointF>> l = new List<List<PointF>>();
                                    List<PointF> p = new List<PointF>();

                                    while ((j < types.Length) && ((types[j] & 0x80) != 0x80))
                                        // p.Add(gp.PathPoints(j))
                                        j += 1;
                                    p.AddRange(gp.PathPoints.Skip(i).Take(j - i));

                                    if (j < gp.PathPoints.Length)
                                        p.Add(gp.PathPoints[j]);
                                    l.Add(p);
                                    i = j;

                                    this.PathList.Add(l);
                                    this.ListBox1.Items.Add("savedPath_" + (this.PathList.Count - 1).ToString());

                                    this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;
                                }
                            }
                            else
                            {
                                gp.Dispose();
                                gp = null;
                            }
                        }
                    }
                    catch
                    {
                        bError = true;
                    }
                }
                else if (tp?.Item1 != null)
                {
                    try
                    {
                        PointF[]? pts2 = tp.Item1; //(PointF[])o[0];
                        byte[]? tps = tp.Item2; //(byte[])o[1];

                        if (pts2 != null && tps != null)
                        {
                            GraphicsPath? gp = new GraphicsPath(pts2, tps);

                            if (this.PathList == null)
                                this.PathList = new List<List<List<PointF>>>();

                            if (gp.PointCount > 1)
                            {
                                byte[] types = gp.PathTypes;
                                for (int i = 0; i <= types.Length - 1; i++)
                                {
                                    int j = i;
                                    List<List<PointF>> l = new List<List<PointF>>();
                                    List<PointF> p = new List<PointF>();

                                    while ((j < types.Length) && ((types[j] & 0x80) != 0x80))
                                        // p.Add(gp.PathPoints(j))
                                        j += 1;
                                    p.AddRange(gp.PathPoints.Skip(i).Take(j - i));

                                    if (j < gp.PathPoints.Length)
                                        p.Add(gp.PathPoints[j]);
                                    l.Add(p);
                                    i = j;

                                    this.PathList.Add(l);
                                    this.ListBox1.Items.Add("savedPath_" + (this.PathList.Count - 1).ToString());

                                    this.ListBox1.SelectedIndex = this.ListBox1.Items.Count - 1;
                                }
                            }
                            else
                            {
                                gp.Dispose();
                                gp = null;
                            }
                        }
                    }
                    catch
                    {
                        bError = true;
                    }
                }

                if (bError)
                    MessageBox.Show("Error.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                this.Enabled = true;
                this.Text = "saved Paths";
                this.Cursor = Cursors.Default;
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (this.CurPath != null)
            {
                List<List<PointF>> fList = ClonePath(this.CurPath, 0);
                if (this.PathList == null)
                    this.PathList = new List<List<List<PointF>>>();
                this.PathList.Add(fList);
                int i = this.ListBox1.Items.Count;
                this.ListBox1.Items.Add("savedPath_" + i.ToString());
            }
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            this.ListBox1.SelectedIndex = -1;
            this.ListBox1_SelectedIndexChanged(this.ListBox1, new EventArgs());
        }

        private void btnReadPath_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false && this.Visible && this.helplineRulerCtrl1.Bmp != null)
            {
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                {
                    if (this.backgroundWorker2.IsBusy)
                        this.backgroundWorker2.CancelAsync();

                    SetControls(false);

                    if (MessageBox.Show("Clear list?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        this.ListBox1.Items.Clear();
                        if (this.PathList == null)
                            this.PathList = new List<List<List<PointF>>>();
                        this.PathList.Clear();
                    }

                    Bitmap bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                    int mOpacity = (int)this.numChainTolerance.Value;

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bmp, mOpacity });
                }
            }
        }

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                if (o == null)
                    return;

                List<ChainCode>? l = new List<ChainCode>();

                using (Bitmap bmp = (Bitmap)o[0])
                {
                    bool transpMode = true;
                    int minOpacity = (int)o[1];
                    l = GetBoundary(bmp, minOpacity, transpMode);

                    if (l != null)
                    {
                        foreach (ChainCode c in l)
                            c.SetId();
                    }
                }

                e.Result = l;
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                {
                    Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                    List<ChainCode> l = (List<ChainCode>)e.Result;

                    ChainFinder cf = new ChainFinder();
                    cf.DrawOutlineToBmp(bmp, this.helplineRulerCtrl1.Bmp, l);

                    if (l != null)
                    {
                        if (this.PathList == null)
                            this.PathList = new List<List<List<PointF>>>();

                    l = l.OrderByDescending((a) => a.Chain.Count).ToList();

                        int cnt = l.Count;
                        if (this.cbRestrict.Checked)
                            cnt = Math.Min(l.Count, (int)this.numRestrict.Value);

                        for (int i = 0; i < cnt; i++)
                        {
                            this.ListBox1.Items.Add("savedPath_" + this.ListBox1.Items.Count.ToString());
                            List<List<PointF>> list = new List<List<PointF>>();
                            List<PointF> inner = new List<PointF>();
                            inner.AddRange(l[i].Coord.Select(a => new PointF(a.X, a.Y)));
                            list.Add(inner);
                            this.PathList.Add(list);
                        }
                    }
                }

                this.SetControls(true);

                this.backgroundWorker2.Dispose();
                this.backgroundWorker2 = new BackgroundWorker();
                this.backgroundWorker2.WorkerReportsProgress = true;
                this.backgroundWorker2.WorkerSupportsCancellation = true;
                this.backgroundWorker2.DoWork += backgroundWorker2_DoWork;
                //this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
                this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private List<ChainCode>? GetBoundary(Bitmap upperImg, int minAlpha, bool transpMode)
        {
            List<ChainCode>? l = null;
            Bitmap? bmpTmp = null;
            try
            {
                if (AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L))
                    bmpTmp = (Bitmap)upperImg.Clone();
                else
                    throw new Exception("Not enough memory.");
                int nWidth = bmpTmp.Width;
                int nHeight = bmpTmp.Height;
                ChainFinder cf = new ChainFinder();

                if (transpMode)
                    lock (this._lockObject)
                        l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, false, 0, false, 0, false);
                else
                    lock (this._lockObject)
                        l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, true, 0, false, 0, false);
            }
            catch (Exception exc)
            {
                OnBoundaryError(exc.Message);
            }
            finally
            {
                if (bmpTmp != null)
                {
                    bmpTmp.Dispose();
                    bmpTmp = null;
                }
            }

            if (l != null)
                return l;
            else
                return null;
        }

        private void OnBoundaryError(string message)
        {
            BoundaryError?.Invoke(this, message);
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.Panel1.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;

                        if (ct is Panel)
                        {
                            ct.Enabled = true;
                            foreach (Control ct2 in ct.Controls)
                                ct2.Enabled = e;
                        }
                    }

                    foreach (Control ct in this.Panel2.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;

                        if (ct is Panel)
                        {
                            ct.Enabled = true;
                            foreach (Control ct2 in ct.Controls)
                                ct2.Enabled = e;
                        }
                    }

                    this.helplineRulerCtrl1.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.Panel1.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;

                    if (ct is Panel)
                    {
                        ct.Enabled = true;
                        foreach (Control ct2 in ct.Controls)
                            ct2.Enabled = e;
                    }
                }

                foreach (Control ct in this.Panel2.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;

                    if (ct is Panel)
                    {
                        ct.Enabled = true;
                        foreach (Control ct2 in ct.Controls)
                            ct2.Enabled = e;
                    }
                }

                this.helplineRulerCtrl1.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }
    }
}
