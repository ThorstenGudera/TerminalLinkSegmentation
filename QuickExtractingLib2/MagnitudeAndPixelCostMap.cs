using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    public class MagnitudeAndPixelCostMap
    {
        public List<double[]>? Ramps { get; set; }

        public double[]? CostMapGrad
        {
            get
            {
                if (Ramps == null || Ramps.Count == 0)
                    return null;
                else
                    return Ramps[0];
            }
        }
        public double[]? CostMapEdge
        {
            get
            {
                if (Ramps == null || Ramps.Count < 1)
                    return null;
                else
                    return Ramps[1];
            }
        }
        public double[]? CostMapIn
        {
            get
            {
                if (Ramps == null || Ramps.Count < 2)
                    return null;
                else
                    return Ramps[2];
            }
        }
        public double[]? CostMapOut
        {
            get
            {
                if (Ramps == null || Ramps.Count < 3)
                    return null;
                else
                    return Ramps[3];
            }
        }
    }
}
