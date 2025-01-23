
namespace OutlineOperations
{
    public class ExcludedBmpRegion : IDisposable
    {
        public Bitmap? Remaining { get; set; }
        public Point Location { get; set; } = new Point(0, 0);

        public ExcludedBmpRegion(Bitmap remaining)
        {
            Remaining = new Bitmap(remaining);
        }

        public void Dispose()
        {
            if (this.Remaining != null)
                this.Remaining.Dispose();
            this.Remaining = null;
        }
    }
}