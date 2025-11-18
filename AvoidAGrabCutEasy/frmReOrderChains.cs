using Cache;
using ChainCodeFinder;
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

namespace AvoidAGrabCutEasy
{
    public partial class frmReOrderChains : Form
    {
        private Bitmap? _bmpBU;
        private List<ChainCode>? _allChains;
        private bool _dontAskOnClosing;
        private List<ChainCode>? _removedChains;
        public Bitmap FBitmap
        {
            get
            {
                return this.helplineRulerCtrl1.Bmp;
            }
        }

        private bool _pic_changed = false;
        private bool _dontDoZoom;
        private int _ix;
        private int _iy;

        public List<ChainCode>? AllChains { get { return _allChains; } }
        public List<ChainCode>? RemovedChains { get { return _removedChains; } }

        public frmReOrderChains()
        {
            InitializeComponent();
        }

        public frmReOrderChains(Bitmap bmp, List<ChainCode> allChains, List<ChainCode> removedChains)
        {
            InitializeComponent();

            //HelperFunctions.HelperFunctions.SetFormSizeBig(this);
            this.CenterToScreen();

            // Me.helplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl2.Bmp = (Bitmap)bmp.Clone();
                this.helplineRulerCtrl1.Bmp = new Bitmap(bmp.Width, bmp.Height);
                _bmpBU = (Bitmap)bmp.Clone();
            }
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }

            this._allChains = allChains;
            this._removedChains = removedChains;

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

            this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
            this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

            this.helplineRulerCtrl2.AddDefaultHelplines();
            this.helplineRulerCtrl2.ResetAllHelpLineLabelsColor();

            this.helplineRulerCtrl1.dbPanel1.MouseDown += helplineRulerCtrl1_MouseDown;
            this.helplineRulerCtrl2.dbPanel1.MouseDown += helplineRulerCtrl2_MouseDown;
            //this.helplineRulerCtrl1.dbPanel1.MouseMove += helplineRulerCtrl1_MouseMove;
            //this.helplineRulerCtrl1.dbPanel1.MouseUp += helplineRulerCtrl1_MouseUp;

            //this.helplineRulerCtrl1.PostPaint += helplineRulerCtrl1_Paint;
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (_ix >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy >= 0 && _iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                FindChain(_ix, _iy);
                DrawChainToResultPic(this.listBox1.SelectedIndex);
            }
        }

        private void helplineRulerCtrl2_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

            if (_ix >= 0 && _ix < this.helplineRulerCtrl2.Bmp.Width && _iy >= 0 && _iy < this.helplineRulerCtrl2.Bmp.Height)
            {
                FindChain(_ix, _iy);
                DrawChainToResultPic(this.listBox1.SelectedIndex);
            }
        }

        private void FindChain(int ix, int iy)
        {
            if (this._allChains != null)
            {
                for (int i = this._allChains.Count - 1; i >= 0; i--)
                {
                    using GraphicsPath gP = new GraphicsPath();
                    List<Point> points = this._allChains[i].Coord;
                    gP.AddLines(points.Select(a => new PointF(a.X, a.Y)).ToArray());
                    if (gP.IsVisible(ix, iy))
                    {
                        this.listBox1.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void frmReOrderChains_Load(object sender, EventArgs e)
        {
            this.listBox1.SuspendLayout();
            this.listBox1.BeginUpdate();
            this._allChains = this._allChains?.OrderByDescending(x => x.Coord.Count).ToList();
            this.listBox1.DataSource = this._allChains;
            this.listBox1.EndUpdate();
            this.listBox1.ResumeLayout();
            this.listBox2.SuspendLayout();
            this.listBox2.BeginUpdate();
            this._removedChains = this._removedChains?.OrderByDescending(x => x.Coord.Count).ToList();
            this.listBox2.DataSource = this._removedChains;
            this.listBox2.EndUpdate();
            this.listBox2.ResumeLayout();
            DrawResultPic();
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbZoom.SelectedIndex = 4;
        }

        private void DrawResultPic()
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._allChains != null && this._bmpBU != null)
            {
                Bitmap bOut = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                this.SetControls(false);
                this.Refresh();

                List<ChainCode>? allChains = this._allChains;
                allChains = allChains?.OrderByDescending(x => x.Coord.Count).ToList();

                if (allChains != null && allChains.Count > 1000)
                    allChains = allChains.Take(1000).ToList();

                //if (allChains != null)
                //    this.numComponents2.Value = this.numComponents2.Maximum = allChains.Count;

                float wFactor = 1.0f; // ((float)bOut.Width + (float)this.numRCX.Value) / (float)bOut.Width;
                float hFactor = 1.0f; // ((float)bOut.Height + (float)this.numRCY.Value) / (float)bOut.Height;

                if (allChains != null)
                    using (TextureBrush tb = new TextureBrush(this._bmpBU))
                    {
                        foreach (ChainCode c in allChains)
                        {
                            using (GraphicsPath gP = new GraphicsPath())
                            {
                                try
                                {
                                    gP.StartFigure();
                                    PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                    gP.AddLines(pts);

                                    using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                        gP.Transform(mx);

                                    using (Graphics gx = Graphics.FromImage(bOut) /*, gx2 = Graphics.FromImage(bC), gx4 = Graphics.FromImage(bCT)*/)
                                    {
                                        gx.FillPath(tb, gP);
                                        //gx2.FillPath(tb, gP);
                                        //gx4.FillPath(tb, gP);
                                    }
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc.ToString());
                                }
                            }
                        }

                        foreach (ChainCode c in allChains)
                        {
                            if (ChainFinder.IsInnerOutline(c))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                            gP.Transform(mx);

                                        using (Graphics gx = Graphics.FromImage(bOut))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }
                    }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                //this.SetBitmap(ref this._bResCopy, ref bC);
                //this.SetBitmap(ref this._bResCopyTransp, ref bCT);

                this._pic_changed = true;

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.SetControls(true);
            }
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer1.Panel1.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;

                        if (ct is GroupBox)
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
                foreach (Control ct in this.splitContainer1.Panel1.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;

                    if (ct is GroupBox)
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

        private void frmReOrderChains_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pic_changed & !_dontAskOnClosing)
            {
                DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                {
                    button2.PerformClick();
                    e.Cancel = true;
                }
                else if (dlg == DialogResult.Cancel)
                    e.Cancel = true;
            }

            if (!e.Cancel)
            {
                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                this._bmpBU = null;
            }
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.cmbZoom.SelectedIndex = 2;
            else if (e.ZoomWidth)
                this.cmbZoom.SelectedIndex = 3;
            else
                this.cmbZoom.SelectedIndex = 4;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            this._dontDoZoom = false;
        }

        private void helplineRulerCtrl2_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.cmbZoom.SelectedIndex = 2;
            else if (e.ZoomWidth)
                this.cmbZoom.SelectedIndex = 3;
            else
                this.cmbZoom.SelectedIndex = 4;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            this._dontDoZoom = false;
        }

        private void SetBitmap(Bitmap bitmapToSet, Bitmap bitmapToBeSet, Control ct, string property)
        {
            Bitmap bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();

            if (ct != null)
            {
                if (ct.GetType().GetProperties().Where((a) => a.Name == property).Count() > 0)
                {
                    System.Reflection.PropertyInfo? pi = ct.GetType().GetProperty(property);
                    pi?.SetValue(ct, bitmapToBeSet);
                }
            }
        }

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (this.helplineRulerCtrl1.Bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.helplineRulerCtrl1.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _pic_changed = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
        }

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = SystemColors.Control;
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(cmbZoom.SelectedItem?.ToString());
                this.helplineRulerCtrl1.Enabled = true;
                if (this.cmbZoom.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex > -1)
            {
                this.label3.Text = this.listBox1.SelectedIndex.ToString();
                DrawChainToPreviewPic(this.listBox1.SelectedIndex);
            }
        }

        private void DrawChainToPreviewPic(int selectedIndex)
        {
            if (this.helplineRulerCtrl2.Bmp != null && selectedIndex > -1 && this._bmpBU != null && this._allChains != null)
            {
                Bitmap bOut = (Bitmap)this._bmpBU.Clone();
                using (TextureBrush tb = new TextureBrush(this._bmpBU))
                {
                    ChainCode c = this._allChains[selectedIndex];
                    float wFactor = 1.0f; // ((float)bOut.Width + (float)this.numRCX.Value) / (float)bOut.Width;
                    float hFactor = 1.0f; // ((float)bOut.Height + (float)this.numRCY.Value) / (float)bOut.Height;

                    using (GraphicsPath gP = new GraphicsPath())
                    {
                        try
                        {
                            gP.StartFigure();
                            PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                            gP.AddLines(pts);

                            using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                gP.Transform(mx);

                            using (Graphics gx = Graphics.FromImage(bOut))
                            {
                                gx.FillPath(tb, gP);
                                using (Pen pen = new Pen(Color.Red, 2))
                                    gx.DrawPath(pen, gP);
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.ToString());
                        }
                    }

                    if (ChainFinder.IsInnerOutline(c))
                        using (GraphicsPath gP = new GraphicsPath())
                        {
                            try
                            {
                                gP.StartFigure();
                                PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                gP.AddLines(pts);

                                using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                    gP.Transform(mx);

                                using (Graphics gx = Graphics.FromImage(bOut))
                                {
                                    gx.CompositingMode = CompositingMode.SourceCopy;
                                    gx.FillPath(Brushes.Transparent, gP);

                                    using (Pen pen = new Pen(Color.Yellow, 2))
                                        gx.DrawPath(pen, gP);
                                }
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.ToString());
                            }
                        }
                }

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");

                this._pic_changed = true;

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void DrawChainToResultPic(int selectedIndex)
        {
            if (this.helplineRulerCtrl1.Bmp != null && selectedIndex > -1 && this._bmpBU != null && this._allChains != null)
            {
                Bitmap bOut = (Bitmap)this._bmpBU.Clone();
                using (TextureBrush tb = new TextureBrush(this._bmpBU))
                {
                    ChainCode c = this._allChains[selectedIndex];
                    float wFactor = 1.0f; // ((float)bOut.Width + (float)this.numRCX.Value) / (float)bOut.Width;
                    float hFactor = 1.0f; // ((float)bOut.Height + (float)this.numRCY.Value) / (float)bOut.Height;

                    using (GraphicsPath gP = new GraphicsPath())
                    {
                        try
                        {
                            gP.StartFigure();
                            PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                            gP.AddLines(pts);

                            using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                gP.Transform(mx);

                            using (Graphics gx = Graphics.FromImage(bOut))
                            {
                                gx.FillPath(tb, gP);
                                using (Pen pen = new Pen(Color.Red, 2))
                                    gx.DrawPath(pen, gP);
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.ToString());
                        }
                    }

                    if (ChainFinder.IsInnerOutline(c))
                        using (GraphicsPath gP = new GraphicsPath())
                        {
                            try
                            {
                                gP.StartFigure();
                                PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                gP.AddLines(pts);

                                using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                    gP.Transform(mx);

                                using (Graphics gx = Graphics.FromImage(bOut))
                                {
                                    gx.CompositingMode = CompositingMode.SourceCopy;
                                    gx.FillPath(Brushes.Transparent, gP);

                                    using (Pen pen = new Pen(Color.Yellow, 2))
                                        gx.DrawPath(pen, gP);
                                }
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.ToString());
                            }
                        }
                }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");

                this._pic_changed = true;

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnRemChainsByArea_Click(object sender, EventArgs e)
        {
            if (this._allChains != null)
            {
                if (this._removedChains == null)
                    this._removedChains = new List<ChainCode>();

                for (int j = this._allChains.Count - 1; j >= 0; j--)
                {
                    if (Math.Abs(this._allChains[j].Area) < this.numRemChainsByArea.Value)
                    {
                        this._removedChains.Insert(0, this._allChains[j]);
                        this._allChains.RemoveAt(j);
                    }
                }

                this.listBox1.DataSource = null;
                this.listBox1.DataSource = this._allChains;
                this.listBox2.DataSource = null;
                this.listBox2.DataSource = this._removedChains;

                if (this._allChains.Count > 0)
                    this.listBox1.SelectedIndex = 0;

                DrawResultPic();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this._allChains != null && this.listBox1.SelectedIndex > -1)
            {
                if (this._removedChains == null)
                    this._removedChains = new List<ChainCode>();

                int j = this.listBox1.SelectedIndex;

                this._removedChains.Insert(0, this._allChains[j]);
                this._allChains.RemoveAt(j);

                this.listBox1.DataSource = null;
                this.listBox1.DataSource = this._allChains;
                this.listBox2.DataSource = null;
                this.listBox2.DataSource = this._removedChains;

                if (this._allChains.Count > j)
                    this.listBox1.SelectedIndex = j;
                else
                    this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;

                DrawResultPic();
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (this._allChains != null && this._removedChains != null && this.listBox2.SelectedIndex > -1)
            {
                int j = this.listBox2.SelectedIndex;

                this._allChains.Add(this._removedChains[j]);
                this._removedChains.RemoveAt(j);

                this._allChains = this._allChains?.OrderByDescending(x => x.Coord.Count).ToList();

                this.listBox1.DataSource = null;
                this.listBox1.DataSource = this._allChains;
                this.listBox2.DataSource = null;
                this.listBox2.DataSource = this._removedChains;

                if (this._allChains?.Count > j)
                    this.listBox1.SelectedIndex = j;
                else
                    this.listBox1.SelectedIndex = 0;

                DrawResultPic();
            }
        }

        private void btnRestoreAll_Click(object sender, EventArgs e)
        {
            if (this._allChains != null && this._removedChains != null && this.listBox2.SelectedIndex > -1)
            {
                for (int j = this._removedChains.Count - 1; j >= 0; j--)
                {
                    this._allChains.Add(this._removedChains[j]);
                    this._removedChains.RemoveAt(j);
                }

                this._allChains = this._allChains?.OrderByDescending(x => x.Coord.Count).ToList();

                this.listBox1.DataSource = null;
                this.listBox1.DataSource = this._allChains;
                this.listBox2.DataSource = null;
                this.listBox2.DataSource = this._removedChains;

                this.listBox1.SelectedIndex = 0;

                DrawResultPic();
            }
        }
    }
}
