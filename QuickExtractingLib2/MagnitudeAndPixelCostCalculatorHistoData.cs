using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    public class MagnitudeAndPixelCostCalculatorHistoData : MagnitudeAndPixelCostCalculator
    {
        public override MagnitudeAndPixelCostMap CalculateRamps()
        {
            if (this.DataGray == null || this.AddressesGray == null || this.BmpDataForValueComputation == null || this.Stride == 0 || this.Dist == 0)
                throw new Exception("The data properties must be set prior to this function call.");

            List<double[]> ramps = new List<double[]>();
            double[]? histo = GetHistoGrad(BmpDataForValueComputation, AddressesGray, true);
            if (histo != null)
            {
                InvertHisto(histo);
                SmoothenHistogram(histo, 9);
                StretchHisto(histo);
                ramps.Add(histo);
            }

            double[]? edges = GetEdges(DataGray, AddressesGray, true);
            if (edges != null)
            {
                InvertHisto(edges);
                SmoothenHistogram(edges, 5);
                StretchHisto(edges);
                ramps.Add(edges);
            }

            List<Tuple<double, double>> normGradients = GetNormalizedGradients(AddressesGray, BmpDataForValueComputation);

            double[]? inPx = GetInPixels(DataGray, AddressesGray, normGradients, Stride, Dist, true);
            if (inPx != null)
            {
                InvertHisto(inPx);
                SmoothenHistogram(inPx, 5);
                StretchHisto(inPx);
                ramps.Add(inPx);
            }

            double[]? outPx = GetOutPixels(DataGray, AddressesGray, normGradients, Stride, Dist, true);
            if (outPx != null)
            {
                InvertHisto(outPx);
                SmoothenHistogram(outPx, 5);
                StretchHisto(outPx);
                ramps.Add(outPx);
            }

            return new MagnitudeAndPixelCostMap() { Ramps = ramps };
        }

        public override MagnitudeAndPixelCostMap? CalculateRamps(byte[] dataGray, List<int> addressesGray, BitmapData bmpDataForValueComputation, int stride, int dist)
        {
            if (this.DataGray == null || this.AddressesGray == null)
                return null;

            List<double[]> ramps = new List<double[]>();
            double[]? histo = GetHistoGrad(bmpDataForValueComputation, addressesGray, true);
            if (histo != null)
            {
                InvertHisto(histo);
                SmoothenHistogram(histo, 5);
                StretchHisto(histo);
                ramps.Add(histo);
            }

            double[]? edges = GetEdges(dataGray, addressesGray, true);
            if (edges != null)
            {
                InvertHisto(edges);
                SmoothenHistogram(edges, 5);
                StretchHisto(edges);
                ramps.Add(edges);
            }

            List<Tuple<double, double>> normGradients = GetNormalizedGradients(addressesGray, bmpDataForValueComputation);

            double[]? inPx = GetInPixels(dataGray, addressesGray, normGradients, stride, dist, true);
            if (inPx != null)
            {
                InvertHisto(inPx);
                SmoothenHistogram(inPx, 5);
                StretchHisto(inPx);
                ramps.Add(inPx);
            }

            double[]? outPx = GetOutPixels(dataGray, addressesGray, normGradients, stride, dist, true);
            if (outPx != null)
            {
                InvertHisto(outPx);
                SmoothenHistogram(outPx, 5);
                StretchHisto(outPx);
                ramps.Add(outPx);
            }

            return new MagnitudeAndPixelCostMap() { Ramps = ramps };
        }

        private List<Tuple<double, double>> GetNormalizedGradients(List<int> addressesGray, BitmapData bmpDataForValueComputation)
        {
            List<Tuple<double, double>> lOut = new List<Tuple<double, double>>();

            unsafe
            {
                byte* p = (byte*)bmpDataForValueComputation.Scan0;

                for (int i = 0; i <= addressesGray.Count - 1; i++)
                {
                    double valAddressX = System.Convert.ToInt32(p[addressesGray[i] * 4]) - 127;
                    double valAddressY = System.Convert.ToInt32(p[addressesGray[i] * 4 + 1]) - 127;

                    valAddressX *= 2;
                    valAddressY *= 2;

                    double abs = Math.Sqrt(valAddressX * valAddressX + valAddressY * valAddressY);
                    double dX = 0;
                    double dY = 0;
                    if (abs > 0)
                    {
                        dX = valAddressX / abs;
                        dY = valAddressY / abs;
                    }

                    lOut.Add(new Tuple<double, double>(dX, dY));
                }
            }
            return lOut;
        }

        private double[]? GetHistoGrad(BitmapData bmpDataForValueComputation, List<int> addressesGray, bool excudeZeroVals)
        {
            double[] histo = new double[256];

            unsafe
            {
                byte* p = (byte*)bmpDataForValueComputation.Scan0;

                try
                {
                    for (int i = 0; i <= 255; i++)
                        histo[i] = 0;

                    for (int i = 0; i <= addressesGray.Count - 1; i++)
                        histo[p[addressesGray[i] * 4 + 2]] += 1;

                    if (excudeZeroVals)
                        histo[0] = 0;

                    double[] histoNorm = new double[256];
                    for (int i = 0; i <= histo.Length - 1; i++)
                        histoNorm[i] = histo[i] / addressesGray.Count;

                    return histoNorm;
                }
                catch
                {
                }
            }

            return null;
        }

        private void InvertHisto(double[] histo)
        {
            for (int i = 0; i <= histo.Length - 1; i++)
                histo[i] = 1.0 - histo[i];
        }

        private double[]? GetEdges(byte[] dataGray, List<int> addressesGray, bool excudeZeroVals)
        {
            double[] fPHisto = new double[256];

            try
            {
                for (int i = 0; i <= 255; i++)
                    fPHisto[i] = 0;

                for (int i = 0; i <= addressesGray.Count - 1; i++)
                    fPHisto[dataGray[addressesGray[i]]] += 1;

                if (excudeZeroVals)
                    fPHisto[0] = 0;

                double[] histoNorm = new double[256];
                for (int i = 0; i <= fPHisto.Length - 1; i++)
                    histoNorm[i] = fPHisto[i] / addressesGray.Count;

                return histoNorm;
            }
            catch
            {
            }

            return null;
        }

        private double[]? GetInPixels(byte[] dataGray, List<int> addressesGray, List<Tuple<double, double>> normalizedGradients, int stride, int dist, bool excudeZeroVals)
        {
            double[] fIHisto = new double[256];

            try
            {
                for (int i = 0; i <= 255; i++)
                    fIHisto[i] = 0;

                int subtract = 0;
                int strideGray = stride / 4;

                for (int i = 0; i <= addressesGray.Count - 1; i++)
                {
                    int fI = 0;

                    double dx = normalizedGradients[i].Item1;
                    double dy = normalizedGradients[i].Item2;

                    if (dx != 0 || dy != 0)
                    {
                        int x = addressesGray[i] % strideGray;
                        int y = addressesGray[i] / strideGray;

                        double fIX = (x + System.Convert.ToInt32(dist * dx));
                        double fIY = (y + System.Convert.ToInt32(dist * dy)) * strideGray;

                        fI = System.Convert.ToInt32(fIY + fIX) >= 0 && System.Convert.ToInt32(fIY + fIX) < dataGray.Length ? dataGray[System.Convert.ToInt32(fIY + fIX)] : 0;

                        fIHisto[fI] += 1;
                    }
                    else
                        subtract += 1;
                }

                if (excudeZeroVals)
                    fIHisto[0] = 0;

                double[] histoNorm = new double[256];
                for (int i = 0; i <= fIHisto.Length - 1; i++)
                    histoNorm[i] = fIHisto[i] / (addressesGray.Count - subtract);

                return histoNorm;
            }
            catch
            {
            }

            return null;
        }

        private double[]? GetOutPixels(byte[] dataGray, List<int> addressesGray, List<Tuple<double, double>> normalizedGradients, int stride, int dist, bool excudeZeroVals)
        {
            double[] fOHisto = new double[256];

            try
            {
                for (int i = 0; i <= 255; i++)
                    fOHisto[i] = 0;

                int subtract = 0;
                int strideGray = stride / 4;

                for (int i = 0; i <= addressesGray.Count - 1; i++)
                {
                    int fO = 0;

                    double dx = normalizedGradients[i].Item1;
                    double dy = normalizedGradients[i].Item2;

                    if (dx != 0 || dy != 0)
                    {
                        int x = addressesGray[i] % strideGray;
                        int y = addressesGray[i] / strideGray;

                        double fOX = (x - System.Convert.ToInt32(dist * dx));
                        double fOY = (y - System.Convert.ToInt32(dist * dy)) * strideGray;

                        fO = System.Convert.ToInt32(fOY + fOX) >= 0 && System.Convert.ToInt32(fOY + fOX) < dataGray.Length ? dataGray[System.Convert.ToInt32(fOY + fOX)] : 0;

                        fOHisto[fO] += 1;
                    }
                    else
                        subtract += 1;
                }

                if (excudeZeroVals)
                    fOHisto[0] = 0;

                double[] histoNorm = new double[256];
                for (int i = 0; i <= fOHisto.Length - 1; i++)
                    histoNorm[i] = fOHisto[i] / (addressesGray.Count - subtract);

                return histoNorm;
            }
            catch
            {
            }

            return null;
        }

        private void SmoothenHistogram(double[] histo, int kernelLength)
        {
            double[] kernel = GetKernelVector(kernelLength);

            double[] histoRead = new double[histo.Length - 1 + 1];
            histo.CopyTo(histoRead, 0);

            int n2 = kernelLength / 2;

            for (int i = 0; i <= histo.Length - 1; i++)
            {
                double sum = 0.0;
                double kernelSum = 0.0;
                int count = 0;

                int vl = i <= n2 ? (n2 - i) : 0;
                int lb = -n2 + vl;
                int vu = (histo.Length - 1 - i) <= n2 ? (n2 - (histo.Length - 1 - i)) : 0;
                int ub = n2 - vu;

                for (int j = lb; j <= ub; j++)
                {
                    sum += histoRead[i + j] * kernel[j + n2];
                    count += 1;
                    kernelSum += kernel[j + n2];
                }

                histo[i] = sum / count * (kernelLength - (vl + vu)) / kernelSum;
            }
        }

        private double[] GetKernelVector(int kernelLength)
        {
            double[] kernelVector = new double[kernelLength - 1 + 1];
            int radius = kernelLength / 2;

            double weight = 0.01;
            if (kernelLength == 3)
                weight = 0.16;

            double a = -2.0 * radius * radius / Math.Log(weight);
            double sum = 0.0;

            for (int x = 0; x <= kernelVector.Length - 1; x++)
            {
                double dist = Math.Abs(x - radius);
                kernelVector[x] = Math.Exp(-dist * dist / a);
                sum += kernelVector[x];
            }

            for (int x = 0; x <= kernelVector.Length - 1; x++)
                kernelVector[x] /= sum;

            return kernelVector;
        }

        private void StretchHisto(double[] histo)
        {
            double min1 = 1.0 - histo.Min();
            double m2y = 1.0 / min1;

            for (int i = 0; i <= histo.Length - 1; i++)
            {
                double x = (1.0 - histo[i]) / min1;
                double f1 = m2y * x;
                histo[i] = 1.0 - (f1 * (1.0 - histo[i]));
            }
        }
    }
}
