namespace AvoidAGrabCutEasy
{
    public class BitmapInfo : IDisposable
    {
        public Bitmap? Bmp { get; set; }
        public int CachePosition { get; set; }

        public BitmapInfo(Bitmap b, int p)
        {
            this.Bmp = b;
            this.CachePosition = p;
        }
        public void Dispose()
        {
            if (this.Bmp != null)
            {
                this.Bmp.Dispose();
                this.Bmp = null;
            }
        }
    }
}