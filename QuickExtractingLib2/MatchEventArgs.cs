namespace QuickExtractingLib2
{
    public class MatchEventArgs : System.EventArgs
    {
        public System.Collections.Generic.List<System.Collections.Generic.List<System.Drawing.PointF>>? CurPath { get; set; }
        public string? msg { get; set; }
        public bool continueWork { get; set; }
        public System.Collections.Generic.List<System.Drawing.PointF>? SeedPoints { get; set; }
        public System.Collections.Generic.List<System.Drawing.PointF>? TempPath { get; set; }
        public System.Collections.Generic.List<double[]>? Ramps {get; set;}
    }
}