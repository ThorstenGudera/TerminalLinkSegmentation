
namespace QuickExtract2
{
    [Serializable]
    public class SavedPath
    {   
        public PointF[]? PathPoints { get; set; }
        public byte[]? PathTypes { get; set; }

        public SavedPath()
        {

        }
    }
}