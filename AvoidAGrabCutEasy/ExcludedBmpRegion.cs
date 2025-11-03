
using ChainCodeFinder;

namespace AvoidAGrabCutEasy
{
    public class ExcludedBmpRegion : IDisposable
    {
        public Bitmap? Remaining { get; set; }
        public Point Location { get; set; } = new Point(0, 0);
        public List<ChainCode>? ChainCode { get; internal set; }

        public ExcludedBmpRegion(Bitmap remaining)
        {
            Remaining = (Bitmap)remaining.Clone();
        }

        public void Dispose()
        {
            if (this.Remaining != null)
                this.Remaining.Dispose();
            this.Remaining = null;
        }
    }
}