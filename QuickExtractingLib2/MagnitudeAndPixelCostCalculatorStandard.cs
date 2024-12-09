using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    public class MagnitudeAndPixelCostCalculatorStandard : MagnitudeAndPixelCostCalculator
    {
        public override MagnitudeAndPixelCostMap CalculateRamps()
        {
            List<double[]> ramps = new List<double[]>();
            double[] d = new double[256];

            for (int i = 0; i <= 255; i++)
                d[i] = (255 - i) / 255.0;
            ramps.Add(d);

            return new MagnitudeAndPixelCostMap() { Ramps = ramps };
        }

        public override MagnitudeAndPixelCostMap CalculateRamps(byte[] dataGray, List<int> addressesGray, BitmapData bmpDataForValueComputation, int stride, int dist)
        {
            List<double[]> ramps = new List<double[]>();
            double[] d = new double[256];

            for (int i = 0; i <= 255; i++)
                d[i] = (255 - i) / 255.0;
            ramps.Add(d);

            return new MagnitudeAndPixelCostMap() { Ramps = ramps };
        }
    }
}
