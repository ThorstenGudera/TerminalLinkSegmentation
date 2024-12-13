
namespace AvoidAGrabCutEasy
{
    public class ExcludedBmpRegion : IDisposable
    {   
        public Bitmap? Remaining { get; }
        public Point Location { get; set; } = new Point(0, 0);

        public ExcludedBmpRegion(Bitmap remaining)
        {
            Remaining = remaining;
        }

        public void Dispose()
        {
            if(this.Remaining != null)
                this.Remaining.Dispose();
        }
    }
}