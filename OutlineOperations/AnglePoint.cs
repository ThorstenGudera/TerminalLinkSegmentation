using System.Drawing;

namespace OutlineOperations
{
    internal class AnglePoint
    {
        public double Angle { get; }
        public PointF Pt { get; }

        public AnglePoint(double angle, PointF ptComp)
        {
            Angle = angle;
            Pt = ptComp;
        }
    }
}