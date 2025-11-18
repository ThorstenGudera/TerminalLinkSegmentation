using Cache;
using LUBitmapDesigner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudoShadow
{
    public partial class frmCloneColors : Form
    {
        private ShapeList? _shapeList;
        private int _ix;
        private int _iy;
        private int _ix2;
        private int _iy2;
        public Bitmap? FBitmap { get; private set; }

        private UndoOPCache? _undoOPCache = null;

        public string? CachePathAddition
        {
            get
            {
                return m_CachePathAddition;
            }
            set
            {
                if (value != null)
                    m_CachePathAddition = value;
            }
        }

        private string? m_CachePathAddition;

        private static int[] CustomColors = new int[] { };
        private bool _pic_changed;
        private Bitmap? _bmpBU;
        private Bitmap? _bmpBU2;
        private bool _dontDoZoom;

        public frmCloneColors(ShapeList? shapeList, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            this._shapeList = shapeList;

            if (this._shapeList != null)
            {
                for (int i = 0; i < this._shapeList.Count; i++)
                    this.cmbSrc.Items.Add(this._shapeList[i].ID.ToString());
                for (int i = 0; i < this._shapeList.Count; i++)
                    this.cmbDest.Items.Add(this._shapeList[i].ID.ToString());

                if (this._shapeList?.Count > 0)
                {
                    //LoadSrcPic(cmbSrc?.Items[0]?.ToString());
                    this.cmbSrc.SelectedIndex = 0;
                    this._bmpBU = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                }

                if (this._shapeList?.Count > 1)
                {
                    //LoadDestPic(cmbDest?.Items[1]?.ToString());
                    this.cmbDest.SelectedIndex = 1;
                    this._bmpBU2 = (Bitmap)this.helplineRulerCtrl2.Bmp.Clone();
                }
                else if (this._shapeList?.Count > 0)
                {
                    //LoadDestPic(cmbDest?.Items[0]?.ToString());
                    this.cmbDest.SelectedIndex = 0;
                    this._bmpBU2 = (Bitmap)this.helplineRulerCtrl2.Bmp.Clone();
                }

                if (this.helplineRulerCtrl1.Bmp != null)
                {
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
                }

                if (this.helplineRulerCtrl2.Bmp != null)
                {
                    double faktor = System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Height);
                    double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Height);
                    if (multiplier >= faktor)
                        this.helplineRulerCtrl2.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Width));
                    else
                        this.helplineRulerCtrl2.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Height));

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.AddDefaultHelplines();
                    this.helplineRulerCtrl2.ResetAllHelpLineLabelsColor();

                    this.helplineRulerCtrl2.dbPanel1.MouseDown += helplineRulerCtrl2_MouseDown;
                    this.helplineRulerCtrl2.dbPanel1.MouseMove += helplineRulerCtrl2_MouseMove;
                }
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (_ix >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy >= 0 && _iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                Color c = this.helplineRulerCtrl1.Bmp.GetPixel(_ix, _iy);
                this.toolStripStatusLabel1.Text = _ix.ToString() + "; " + _iy.ToString();
                this.toolStripStatusLabel2.BackColor = c;
            }
        }

        private void helplineRulerCtrl2_MouseMove(object? sender, MouseEventArgs e)
        {
            _ix2 = System.Convert.ToInt32((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
            _iy2 = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

            if (_ix2 >= 0 && _ix2 < this.helplineRulerCtrl2.Bmp.Width && _iy2 >= 0 && _iy2 < this.helplineRulerCtrl2.Bmp.Height)
            {
                Color c = this.helplineRulerCtrl2.Bmp.GetPixel(_ix2, _iy2);
                this.toolStripStatusLabel3.Text = _ix2.ToString() + "; " + _iy2.ToString();
                this.toolStripStatusLabel4.BackColor = c;
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmClCol2");
            if (this._shapeList != null && this._shapeList.Count > 1)
                _undoOPCache.Add(this._shapeList[1].Bmp);
            else if (this._shapeList != null && this._shapeList.Count > 0)
                _undoOPCache.Add(this._shapeList[0].Bmp);
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (_ix >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy >= 0 && _iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                this.numSrcX.Value = (decimal)_ix;
                this.numSrcY.Value = (decimal)_iy;

                Color c = this.helplineRulerCtrl1.Bmp.GetPixel(_ix, _iy);
                this.lblSrcCol.BackColor = c;
            }
        }

        private void helplineRulerCtrl2_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix2 = System.Convert.ToInt32((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
            _iy2 = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

            if (_ix2 >= 0 && _ix2 < this.helplineRulerCtrl2.Bmp.Width && _iy2 >= 0 && _iy2 < this.helplineRulerCtrl2.Bmp.Height)
            {
                this.numDestX.Value = (decimal)_ix2;
                this.numDestY.Value = (decimal)_iy2;

                Color c = this.helplineRulerCtrl2.Bmp.GetPixel(_ix2, _iy2);
                this.lblDestCol.BackColor = c;
            }
        }

        private void frmCloneColors_Load(object sender, EventArgs e)
        {
            if (this._shapeList != null)
            {
                this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
                this.cmbZoom.SelectedIndex = 4;
            }
        }

        private void cmbSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSrcPic(cmbSrc?.SelectedItem?.ToString());
            this.numSrcX.Value = this.numSrcY.Value = 0;
        }

        private void LoadSrcPic(string? v)
        {
            int j = -1;

            if (Int32.TryParse(v, out j))
            {
                BitmapShape? bs = this._shapeList?.GetShapeById(j);
                if (bs != null)
                {
                    Bitmap? b = bs.Bmp;
                    if (b != null)
                    {
                        Bitmap? bmp = (Bitmap)b.Clone();

                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                        if (this.cmbZoom?.SelectedItem != null)
                            this.helplineRulerCtrl1.SetZoom(this.cmbZoom?.SelectedItem?.ToString());

                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        if (this.numSrcX.Value >= this.helplineRulerCtrl1.Bmp.Width)
                            this.numSrcX.Value = this.numSrcX.Maximum = this.helplineRulerCtrl1.Bmp.Width - 1;
                        this.numSrcX.Maximum = this.helplineRulerCtrl1.Bmp.Width - 1;

                        if (this.numSrcY.Value >= this.helplineRulerCtrl1.Bmp.Height)
                            this.numSrcY.Value = this.numSrcY.Maximum = this.helplineRulerCtrl1.Bmp.Height - 1;
                        this.numSrcY.Maximum = this.helplineRulerCtrl1.Bmp.Height - 1;
                    }
                }

            }
        }

        private void cmbDest_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDestPic(cmbDest?.SelectedItem?.ToString());
            this.numDestX.Value = this.numDestY.Value = 0;

            if (this._undoOPCache != null)
                this._undoOPCache.Clear(this.helplineRulerCtrl2.Bmp);

            this.btnUndo.Enabled = this.btnRedo.Enabled = false;
        }

        private void LoadDestPic(string? v)
        {
            int j = -1;

            if (Int32.TryParse(v, out j))
            {
                BitmapShape? bs = this._shapeList?.GetShapeById(j);
                if (bs != null)
                {
                    Bitmap? b = bs.Bmp;
                    if (b != null)
                    {
                        Bitmap? bmp = (Bitmap)b.Clone();

                        this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                        this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                        if (this.cmbZoom?.SelectedItem != null)
                            this.helplineRulerCtrl2.SetZoom(this.cmbZoom?.SelectedItem?.ToString());

                        this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                        this.helplineRulerCtrl2.dbPanel1.Invalidate();

                        if (this.numDestX.Value >= this.helplineRulerCtrl2.Bmp.Width)
                            this.numDestX.Value = this.numDestX.Maximum = this.helplineRulerCtrl2.Bmp.Width - 1;
                        this.numDestX.Maximum = this.helplineRulerCtrl2.Bmp.Width - 1;

                        if (this.numDestY.Value >= this.helplineRulerCtrl2.Bmp.Height)
                            this.numDestY.Value = this.numDestY.Maximum = this.helplineRulerCtrl2.Bmp.Height - 1;
                        this.numDestY.Maximum = this.helplineRulerCtrl2.Bmp.Height - 1;
                    }
                }

            }
        }

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = SystemColors.Control;
        }

        private void cmbZoom_SelectedIndexChanged(object? sender, EventArgs e)
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
            if (this.Visible && this.helplineRulerCtrl2.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl2.Enabled = false;
                this.helplineRulerCtrl2.Refresh();
                this.helplineRulerCtrl2.SetZoom(cmbZoom.SelectedItem?.ToString());
                this.helplineRulerCtrl2.Enabled = true;
                if (this.cmbZoom.SelectedIndex < 2)
                    this.helplineRulerCtrl2.ZoomSetManually = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = this.helplineRulerCtrl1.Bmp;

                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _pic_changed = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                Bitmap? bmp = this.helplineRulerCtrl2.Bmp;

                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _pic_changed = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.I | Keys.Control | Keys.Shift))
            {
                if (this.helplineRulerCtrl1.Bmp != null)
                    MessageBox.Show(this.helplineRulerCtrl1.Bmp.Size.ToString());

                return true;
            }

            if (keyData == Keys.Enter)
            {
                this.helplineRulerCtrl1._contentPanel_MouseDoubleClick(this.helplineRulerCtrl1.dbPanel1, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, 0, 0, 0));

                return true;
            }

            if (keyData == (Keys.Z | Keys.Control))
            {
                this.btnUndo.PerformClick();
                return true;
            }

            if (keyData == (Keys.Y | Keys.Control))
            {
                //this.btnRedo.PerformClick();
                return true;
            }

            //if (keyData == Keys.F3)
            //{
            //    if (this.FrmSetRamFreeable != null && this.FrmSetRamFreeable.Visible)
            //    {
            //        this.FrmSetRamFreeable.Show();
            //        this.FrmSetRamFreeable.BringToFront();
            //    }
            //    else
            //    {
            //        this.FrmSetRamFreeable = new frmSetRamFreeableAmount(this);
            //        this.FrmSetRamFreeable.Show();
            //    }
            //}

            return base.ProcessCmdKey(ref msg, keyData);
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

        private void SetBitmap(ref Bitmap bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _pic_changed = false;
            this.FBitmap = this.helplineRulerCtrl2.Bmp;
        }

        private void frmCloneColors_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pic_changed)
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
                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                if (this._bmpBU2 != null)
                    this._bmpBU2.Dispose();
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

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoUndo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");
                    if (_undoOPCache.CurrentPosition < 1)
                    {
                        this.btnUndo.Enabled = false;
                        this._pic_changed = false;
                    }

                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    this.CheckRedoButton();

                    this.btnUndo.Enabled = this._undoOPCache.CurrentPosition > 1;
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoRedo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = true;

                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    this.CheckRedoButton();

                    this.btnUndo.Enabled = true;
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
        }

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void numSrcX_ValueChanged(object sender, EventArgs e)
        {
            Color c = this.helplineRulerCtrl1.Bmp.GetPixel((int)this.numSrcX.Value, _iy);
            this.lblSrcCol.BackColor = c;
        }

        private void numSrcY_ValueChanged(object sender, EventArgs e)
        {
            Color c = this.helplineRulerCtrl1.Bmp.GetPixel(_ix, (int)this.numSrcY.Value);
            this.lblSrcCol.BackColor = c;
        }

        private void numDestX_ValueChanged(object sender, EventArgs e)
        {
            Color c = this.helplineRulerCtrl2.Bmp.GetPixel((int)this.numDestX.Value, _iy2);
            this.lblDestCol.BackColor = c;
        }

        private void numDestY_ValueChanged(object sender, EventArgs e)
        {
            Color c = this.helplineRulerCtrl2.Bmp.GetPixel(_ix2, (int)this.numDestY.Value);
            this.lblDestCol.BackColor = c;
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                SetControls(false);
                this.btnClone.Enabled = true;
                this.btnClone.Text = "Cancel";

                Color orgCol = this.lblDestCol.BackColor;
                Color replCol = this.lblSrcCol.BackColor;

                Bitmap b = (Bitmap)this.helplineRulerCtrl2.Bmp.Clone();

                this.backgroundWorker1.RunWorkerAsync(new object[] { b, orgCol, replCol });
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
                    }

                    foreach (Control ct in this.splitContainer4.Panel2.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;
                    }

                    this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl2.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.splitContainer1.Panel1.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;
                }

                foreach (Control ct in this.splitContainer4.Panel2.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;
                }

                this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl2.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap b = (Bitmap)o[0];
                Color orgCol = (Color)o[1];
                Color replCol = (Color)o[2];

                PseudoShadow.Fipbmp.ReplaceColors(b, replCol.A, replCol.R, replCol.G, replCol.B, 255,
                    orgCol.A, orgCol.R, orgCol.G, orgCol.B);

                e.Result = b;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (!this.IsDisposed && this.Visible)
                {
                    Bitmap? bmpNew = null;

                    try
                    {
                        bmpNew = (Bitmap)e.Result;

                        if (bmpNew != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmpNew, this.helplineRulerCtrl2, "Bmp");
                            _undoOPCache?.Add(this.helplineRulerCtrl2.Bmp);
                            this.btnUndo.Enabled = true;
                            CheckRedoButton();
                            this._pic_changed = true;
                        }
                    }
                    catch
                    {
                    }

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                    this.helplineRulerCtrl2.CalculateZoom();
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    SetControls(true);

                    this.Cursor = Cursors.Default;
                    this.btnClone.Text = "CloneColors";

                    // create a new bgw, since it reported "complete"
                    this.backgroundWorker1.Dispose();
                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.WorkerReportsProgress = true;
                    this.backgroundWorker1.WorkerSupportsCancellation = true;
                    this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                    this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                }
            }
        }
    }
}
