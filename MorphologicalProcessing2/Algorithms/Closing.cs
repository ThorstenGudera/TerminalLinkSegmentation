using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MorphologicalProcessing2.Algorithms
{
    public class Closing : IMorphologicalOperation
    {
        public int[,]? Kernel { get; set; }

        public Bitmap? KernelBmp { get; set; }
        public void Dispose()
        {
            if (this.KernelBmp != null)
            {
                this.KernelBmp.Dispose();
                this.KernelBmp = null;
            }
        }

        public bool RotateDilationKernels { get; set; }

        public BackgroundWorker? BGW { get; set; }

        public void ApplyGrayscale(Bitmap bmp)
        {              
            Dilate dilate = new Dilate();
            dilate.Kernel = this.Kernel;
            dilate.RotateDilationKernels = this.RotateDilationKernels;
            dilate.BGW = this.BGW;
            dilate.ApplyGrayscale(bmp);

            Erode erode = new Erode();
            erode.Kernel = this.Kernel;
            erode.BGW = this.BGW;
            erode.ApplyGrayscale(bmp);
        }

        public bool Setup(int width, int height)
        {
            this.Kernel = new int[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    this.Kernel[x, y] = 1;

            return true;
        }

        public bool SetupEx(int width, int height)
        {
            this.Kernel = new int[width, height];
            double radiusA = width / 2.0;
            double radiusB = height / 2.0;

            double cntrX = radiusA;
            double cntrY = radiusB;

            double numEx = 1.0;

            if (radiusA > 0 || radiusB > 0)
            {
                if (radiusA >= radiusB)
                    numEx = Math.Sqrt((radiusA * radiusA) - (radiusB * radiusB)) / radiusA;
                else
                    numEx = Math.Sqrt((radiusB * radiusB) - (radiusA * radiusA)) / radiusB;
            }

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double xAb = x - cntrX;
                    double yAb = y - cntrY;
                    double theta = Math.Atan2(yAb, xAb);

                    double radius = Math.Sqrt(xAb * xAb + yAb * yAb);
                    double rMax = 0.0;

                    rMax = radiusB / Math.Sqrt(1.0 - numEx * numEx * Math.Cos(theta) * Math.Cos(theta));
                    if (radiusA < radiusB)
                    {
                        double theta2 = Math.Atan2(xAb, yAb);
                        rMax = radiusA / Math.Sqrt(1.0 - numEx * numEx * Math.Cos(theta2) * Math.Cos(theta2));
                    }

                    if (radius <= rMax)
                        this.Kernel[x, y] = 1;
                }

            return true;
        }
    }
}
