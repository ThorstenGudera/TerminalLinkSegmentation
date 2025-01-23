using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    //roughly following:
    //Interactive Segmentation with Intelligent Scissors
    //Eric N.Mortensen
    //William A.Barrett#
    //https://courses.cs.washington.edu/courses/cse455/09wi/readings/seg_scissors.pdf
    public class PicAlg : IDisposable
    {
        public MagnitudeAndPixelCostMap? CostMaps { get; set; } = null;
        public MagnitudeAndPixelCostCalculator? Mcc { get; set; } = null;

        public byte[]? GrayRepresentation { get; set; } = null;
        public int StrideGray { get; set; } = 0;

        public BitmapData? bmpDataForValueComputation { get; internal set; }

        internal virtual Tuple<double, bool> ComputeValueToNeighbor(BitmapData bmpDataForValueComputation, 
            int address, int neighbor, int stride, double valL, double valM, double valG, int laplaceUpperThreshold, 
            bool procNeighbor, int dist, double edgeWeight, double scale, bool useProcNeighbor, 
            double valP, double valI, double valO, double valCl, double valCol)
        {
            int x = address % stride;
            int y = address / stride;
            int x2 = neighbor % stride;
            int y2 = neighbor / stride;
            int xd = (x2 - x) / 4;
            int yd = y2 - y;
            return new Tuple<double, bool>(System.Convert.ToSingle(Math.Sqrt(xd * xd + yd * yd)), true);
        }

        internal virtual byte[] InitBmpDataForValueComputation(BitmapData data, int stride, bool doR, bool doG, bool doB)
        {
            int l = data.Height * stride;
            byte[] f = new byte[l];
            return f;
        }

        internal void SetVariablesToCostCalculator(List<int> addresses, byte[] p, BitmapData bmpDataForValueComputation, int stride, int dist)
        {
            if (p == null || addresses == null || bmpDataForValueComputation == null || stride == 0 || dist == 0)
                throw new Exception("All parameters must be set.");

            if (Mcc == null)
                throw new Exception("MagnitudeAndPixelCostCalculator is Nothing.");

            Mcc.AddressesGray = addresses;
            Mcc.DataGray = p;
            Mcc.BmpDataForValueComputation = bmpDataForValueComputation;
            Mcc.Stride = stride;
            Mcc.Dist = dist;
        }

        private bool disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        // Protected Overrides Sub Finalize()
        // ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
        }
    }
}
