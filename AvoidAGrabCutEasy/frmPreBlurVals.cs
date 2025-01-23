using ConvolutionLib;
using HelplineRulerControl;
using SegmentsListLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmPreBlurVals : Form
    {
        private Bitmap? _fBitmap;
        public Bitmap? FBitmap
        {
            get
            {
                return _fBitmap;
            }
        }

        public bool ValsChanged { get; internal set; } = false;

        public frmPreBlurVals(Bitmap bmp)
        {
            InitializeComponent();

            this.pictureBox1.Image = bmp;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.ValsChanged = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                Bitmap b = new Bitmap(this.pictureBox1.Image);

                bool blur = this.cbBlur.Checked;
                bool colors = this.cbColors.Checked;
                bool blurFirst = this.rbBefore.Checked;

                int krnl = (int)this.numKernel.Value;
                int maxVal = (int)this.numDistWeight.Value;
                Point pt = new Point((int)this.numValSrc.Value, (int)this.numValDst.Value);

                bool doIgg = this.cbIGG.Checked;

                int kernelLength = (int)numIGGKernel.Value;
                double alpha = (double)this.numIGGAlpha.Value * 255.0;
                double divisor = (double)this.numIGGDivisor.Value;

                this.progressBar1.Value = 0;

                object[] o = { b, blur, colors, blurFirst, krnl, maxVal, pt, doIgg, kernelLength, alpha, divisor };

                this.backgroundWorker1.RunWorkerAsync(o);
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                using Bitmap bOrig = (Bitmap)o[0];

                Bitmap bmp = new Bitmap(bOrig.Width, bOrig.Height);
                using Graphics gx = Graphics.FromImage(bmp);
                gx.DrawImage(bOrig, 0, 0, bmp.Width, bmp.Height);

                bool blur = (bool)o[1];
                bool colors = (bool)o[2];
                bool blurFirst = (bool)o[3];

                int krnl = (int)o[4];
                int maxVal = (int)o[5];
                Point pt = (Point)o[6];

                bool doIgg = (bool)o[7];
                int kernelLength = (int)o[8];
                double cornerWeight = 0.01;
                int sigma = 255;
                double steepness = 1E-12;
                int radius = 340;
                double alpha = (double)o[9];
                GradientMode gradientMode = GradientMode.Scharr16;
                double divisor = (double)o[10];
                bool stretchValues = true;
                int threshold = 127;

                Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                if (blur && !colors)
                    DoBlur(bmp, krnl, maxVal);
                if (!blur && colors)
                    DoColors(bmp, pt);
                if (blur && colors)
                {
                    if (blurFirst)
                    {
                        DoBlur(bmp, krnl, maxVal);
                        DoColors(bmp, pt);
                    }
                    else
                    {
                        DoColors(bmp, pt);
                        DoBlur(bmp, krnl, maxVal);
                    }
                }

                if (doIgg)
                {
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (r.Width > 0 && r.Height > 0)
                        {
                            InvGaussGradOp igg = new InvGaussGradOp();
                            igg.BGW = this.backgroundWorker1;
                            Bitmap? iG = igg.Inv_InvGaussGrad(bmp, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                                sigma, steepness, radius, stretchValues, threshold);
                            e.Result = iG;
                        }
                    }
                }
                else
                    e.Result = bmp;
            }
        }

        private void DoBlur(Bitmap bmp, int krnl, int maxVal)
        {
            Convolution conv = new();
            conv.ProgressPlus += Conv_ProgressPlus;
            conv.CancelLoops = false;

            InvGaussGradOp igg = new InvGaussGradOp();
            igg.BGW = this.backgroundWorker1;

            igg.FastZGaussian_Blur_NxN_SigmaAsDistance(bmp, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);
            conv.ProgressPlus -= Conv_ProgressPlus;
        }

        private void DoColors(Bitmap bmp, Point pt)
        {
            byte[] rgb = new byte[256];
            List<Point> p = new();
            p.Add(new Point(0, 0));
            p.Add(pt);
            p.Add(new Point(255, 255));

            CurveSegment cuSgmt = new();
            List<BezierSegment> bz = cuSgmt.CalcBezierSegments(p.ToArray(), 0.5f);
            List<PointF> pts = cuSgmt.GetAllPoints(bz, 256, 0, 255);
            cuSgmt.MapPoints(pts, rgb);

            ColorCurves.fipbmp.GradColors(bmp, rgb, rgb, rgb);
        }

        private void Conv_ProgressPlus(object sender, ProgressEventArgs e)
        {
            this.backgroundWorker1.ReportProgress((int)e.CurrentProgress);
        }

        private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= this.progressBar1.Maximum)
                this.progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                using Bitmap bmp = (Bitmap)e.Result;

                if (bmp != null)
                {
                    Bitmap bRef = new Bitmap(bmp);
                    Image? iOld = this.pictureBox2.Image;
                    this.pictureBox2.Image = bRef;
                    if (iOld != null)
                        iOld.Dispose();
                    iOld = null;
                    this.pictureBox2.Refresh();

                    Bitmap b = new Bitmap(bmp.Width, bmp.Height);
                    using Graphics g = Graphics.FromImage(b);
                    g.DrawImage(this.pictureBox1.Image, 0, 0, b.Width, b.Height);
                    using Graphics gx = Graphics.FromImage(b);

                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = (float)this.numOpacity.Value;

                    using ImageAttributes ia = new ImageAttributes();

                    ia.SetColorMatrix(cm);
                    gx.DrawImage(bmp,
                                new Rectangle(0, 0, b.Width, b.Height),
                                0,
                                0,
                                b.Width,
                                b.Height, GraphicsUnit.Pixel, ia);

                    Image? iOld2 = this.pictureBox3.Image;
                    this.pictureBox3.Image = b;
                    if (iOld2 != null)
                        iOld2.Dispose();
                    iOld2 = null;
                    this.pictureBox3.Refresh();

                    this.progressBar1.Value = 0;
                }

                this.backgroundWorker1.Dispose();
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorker1.WorkerReportsProgress = true;
                this.backgroundWorker1.WorkerSupportsCancellation = true;
                this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.splitContainer2.Panel1.Focus();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.splitContainer2.Panel2.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.pictureBox3.Image != null || (this.ValsChanged && this.pictureBox3.Image != null))
                this._fBitmap = new Bitmap(this.pictureBox3.Image);
        }

        private void numIGGKernel_ValueChanged(object sender, EventArgs e)
        {
            this.ValsChanged = true;
        }

        private void cbVarLog_CheckedChanged(object sender, EventArgs e)
        {
            this.ValsChanged = true;
        }

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.pictureBox2.BackColor = this.pictureBox3.BackColor = SystemColors.ControlDarkDark;
            else
                this.pictureBox2.BackColor = this.pictureBox3.BackColor = SystemColors.Control;
        }

        private void frmPreBlurVals_Load(object sender, EventArgs e)
        {
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
        }
    }
}
