using ConvolutionLib;
using HelplineRulerControl;
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
    public partial class frmIGGVals : Form
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

        public frmIGGVals(Bitmap bmp)
        {
            InitializeComponent();

            this.pictureBox1.Image = bmp;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbIGG.Checked)
            {
                this.groupBox1.Enabled = true;
                this.groupBox2.Enabled = false;
            }
            else
            {
                this.groupBox1.Enabled = false;
                this.groupBox2.Enabled = true;
            }
            this.ValsChanged = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                Bitmap b = new Bitmap(this.pictureBox1.Image);
                int kernelLength = (int)this.numIGGKernel.Value;
                double cornerWeight = 0.01;
                int sigma = 255;
                double steepness = 1E-12;
                int radius = 340;
                double alpha = (double)this.numIGGAlpha.Value * 255.0;
                GradientMode gradientMode = GradientMode.Scharr16;
                double divisor = (double)this.numIGGDivisor.Value;
                bool grayscale = false;
                bool stretchValues = true;
                int threshold = 127;
                bool replaceBG = this.cbReplaceBG.Checked;
                int replaceTol = (int)this.numReplace.Value;

                int numVarKernel = (int)this.numVarKernel.Value;
                int numVarExpander = (int)this.numVarExpander.Value;
                int numVarTolerance = (int)this.numVarTolerance.Value;
                bool numVarLog = this.cbVarLog.Checked;
                double numVarGamma = (double)this.numVarGamma.Value;

                bool igg = this.rbIGG.Checked;

                this.progressBar1.Value = 0;

                object[] o = { kernelLength, cornerWeight, sigma, steepness,
                               radius, alpha, gradientMode, divisor, grayscale, stretchValues,
                               threshold, replaceBG, replaceTol, numVarKernel, numVarExpander,
                               numVarTolerance, igg, numVarLog, numVarGamma, b};

                this.backgroundWorker1.RunWorkerAsync(o);
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                using Bitmap bOrig = (Bitmap)o[19];

                using Bitmap bmp = new Bitmap(bOrig.Width, bOrig.Height);
                using Graphics gx = Graphics.FromImage(bmp);
                gx.DrawImage(bOrig, 0, 0, bmp.Width, bmp.Height);

                int kernelLength = (int)o[0];
                double cornerWeight = (double)o[1];
                int sigma = (int)o[2];
                double steepness = (double)o[3];
                int radius = (int)o[4];
                double alpha = (double)o[5];
                GradientMode gradientMode = (GradientMode)o[6];
                double divisor = (double)o[7];
                bool grayscale = (bool)o[8];
                bool stretchValues = (bool)o[9];
                int threshold = (int)o[10];
                bool replaceBG = (bool)o[11];
                int replaceTol = (int)o[12];
                int numVarKernel = (int)o[13];
                int numVarExpander = (int)o[14];
                int numVarTolerance = (int)o[15];
                bool doIGG = (bool)o[16];

                bool log = (bool)o[17];
                double gamma = (double)o[18];

                Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                if (doIGG)
                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (r.Width > 0 && r.Height > 0)
                        {
                            InvGaussGradOp igg = new InvGaussGradOp();
                            igg.BGW = this.backgroundWorker1;
                            Bitmap? iG = igg.Inv_InvGaussGrad(bmp, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                                sigma, steepness, radius, stretchValues, threshold);

                            if (replaceBG && iG != null)
                                EdgeDetectionMethods.ReplaceColors(iG, 0, 0, 0, 0, replaceTol, 255, 0, 0, 0);
                            e.Result = iG;
                        }
                    }
                else
                {
                    EdgeDetectionMethods edd = new EdgeDetectionMethods();
                    using Bitmap? bmpSquares = edd.ComputeProductsOfImage(bmp);

                    Convolution conv = new();
                    conv.CancelLoops = false;

                    conv.ProgressPlus += Conv_ProgressPlus;

                    bool b = false;

                    if (bmpSquares != null)
                    {
                        b = edd.FastZBinomial_Blur_NxN(bmpSquares, numVarKernel, 0.01, 255, false, false, conv, this.backgroundWorker1, log);

                        if (b)
                        {
                            b = false;
                            b = edd.FastZBinomial_Blur_NxN(bmp, numVarKernel, 0.01, 255, false, false, conv, this.backgroundWorker1, log);

                            if (b)
                            {
                                using Bitmap? bmpMean = edd.ComputeProductsOfImage(bmp);
                                Bitmap? bVar = null;
                                if (bmpMean != null)
                                    bVar = edd.Subtract(bmpSquares, bmpMean, numVarExpander / 128.0, 1.0 / (gamma == 0 ? 1 : gamma));

                                if (bVar != null)
                                    EdgeDetectionMethods.ReplaceColors(bVar, 0, 0, 0, 0, numVarTolerance, 255, 0, 0, 0);

                                e.Result = bVar;
                            }
                        }
                    }

                    conv.ProgressPlus -= Conv_ProgressPlus;
                }
            }
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
    }
}
