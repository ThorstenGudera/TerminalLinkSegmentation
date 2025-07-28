using ChainCodeFinder;
using System.Collections;

namespace QuickExtract2
{
    internal class Fipbmp
    {
        public static Bitmap? RemOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
            {
                Bitmap? b = null;
                BitArray? fbits = null;

                try
                {
                    b = (Bitmap)bmp.Clone();

                    for (int i = 0; i <= breite - 1; i++)
                    {
                        if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                            break;

                        ChainFinder cf = new ChainFinder();
                        cf.AllowNullCells = true;

                        List<ChainCode> fList = cf.GetOutline(b, 0, false, 0, false);

                        cf.RemoveOutline(b, fList);

                        fbits = null;
                    }

                    return b;
                }
                catch
                {
                    if (fbits != null)
                        fbits = null;
                    if (b != null)
                        b.Dispose();

                    b = null;
                }
            }

            return null;
        }

        internal static List<ChainCode> GetOutline(Bitmap bmp, int alphaMax)
        {
            ChainFinder cf = new ChainFinder();
            List<ChainCode> c = cf.GetOutline(bmp, alphaMax, false, 0, false, 0, false);
            c = c.OrderByDescending(a => a.Area).ToList();

            return c;
        }
    }
}