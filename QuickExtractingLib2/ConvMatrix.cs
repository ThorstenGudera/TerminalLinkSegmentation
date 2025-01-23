using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickExtractingLib2
{
    using System;

    internal class ConvMatrix : ICloneable
    {
        public int TopLeft = 0;
        public int TopMid = 0;
        public int TopRight = 0;
        public int MidLeft = 0;
        public int Pixel = 1;
        public int MidRight = 0;
        public int BottomLeft = 0;
        public int BottomMid = 0;
        public int BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int nVal)
        {
            TopLeft = InlineAssignHelper(ref TopMid, InlineAssignHelper(ref TopRight, InlineAssignHelper(ref MidLeft, InlineAssignHelper(ref Pixel, InlineAssignHelper(ref MidRight, InlineAssignHelper(ref BottomLeft, InlineAssignHelper(ref BottomMid, InlineAssignHelper(ref BottomRight, nVal))))))));
        }
        private static T InlineAssignHelper<T>(ref T target, T value)
        {
            target = value;
            return value;
        }

        public object Clone()
        {
            ConvMatrix cm = new ConvMatrix();
            cm.BottomLeft = this.BottomLeft;
            cm.BottomMid = this.BottomMid;
            cm.BottomRight = this.BottomRight;
            cm.Factor = this.Factor;
            cm.MidLeft = this.MidLeft;
            cm.MidRight = this.MidRight;
            cm.Offset = this.Offset;
            cm.Pixel = this.Pixel;
            cm.TopLeft = this.TopLeft;
            cm.TopMid = this.TopMid;
            cm.TopRight = this.TopRight;
            return cm;
        }
    }

}
