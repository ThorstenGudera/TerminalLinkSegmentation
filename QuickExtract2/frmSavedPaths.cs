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

        public frmSavedPaths(Bitmap bmp, List<List<PointF>> curPath, List<List<List<PointF>>>? pathList)
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
                        int x = this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                        int y = this.HelplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                        using (Matrix m = new Matrix(this.HelplineRulerCtrl1.Zoom, 0, 0, this.HelplineRulerCtrl1.Zoom, x, y))
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
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1 && this.PathList != null)
            {
                this._selectedPath = this.PathList[this.ListBox1.SelectedIndex];
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
            else
            {
                this._selectedPath = null;
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
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
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.HelplineRulerCtrl1.Bmp != null)
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
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
                    this.HelplineRulerCtrl1.dbPanel1.Invalidate();

                    if (this.CheckBox3.Checked)
                    {
                        List<List<PointF>> fList = ClonePath(this.CurPath, 0);
                        this.PathList.Add(fList);
                        int i = this.ListBox1.Items.Count;
                        this.ListBox1.Items.Add("savedPath_" + i.ToString());
                    }
                }
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.Label4.Enabled = this.CheckBox2.Checked;
            this.Button8.Enabled = this.CheckBox2.Checked;
            this.HelplineRulerCtrl1.dbPanel1.Invalidate();
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
                this.HelplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.HelplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
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
                this.HelplineRulerCtrl1.dbPanel1.Invalidate();
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

                if (sp != null)
                {
                    try
                    {
                        PointF[]? pts2 = sp.PathPoints; //(PointF[])o[0];
                        byte[]? tps = sp.PathTypes; //(byte[])o[1];

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
    }
}
