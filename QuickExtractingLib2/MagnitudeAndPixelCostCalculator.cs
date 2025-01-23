using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    public abstract class MagnitudeAndPixelCostCalculator
    {
        public List<int>? AddressesGray { get; set; }
        public byte[]? DataGray { get; set; }
        public BitmapData? BmpDataForValueComputation { get; set; }
        public int Stride { get; set; }
        public int Dist { get; set; }

        public abstract MagnitudeAndPixelCostMap? CalculateRamps();
        public abstract MagnitudeAndPixelCostMap? CalculateRamps(byte[] dataGray, List<int> addressesGray, BitmapData bmpDataForValueComputation, int stride, int dist);
    }
}
