using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtract2
{
    public class PointDataPath
    {
        public int Index { get; set; }
        public double Distance { get; set; }

        public override string ToString()
        {
            return Index.ToString() + ", " + Distance.ToString();
        }
    }
}
