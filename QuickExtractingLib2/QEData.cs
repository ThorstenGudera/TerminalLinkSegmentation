using System.Collections.Generic;

namespace QuickExtractingLib2
{
    public class QEData
    {
        public QEData()
        {

        }

        public QEData(int weight, Stack<int> addresses)
        {
            this.Weight = weight;
            this.Adresses = addresses;
        }

        public int Weight { get; set; }
        public Stack<int>? Adresses { get; private set; }

        public int Index { get; set; }
    }
}