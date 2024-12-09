namespace QuickExtractingLib2
{
    public class NotifyEventArgs : System.EventArgs
    {
        public string? msg { get; set; }
        public System.Collections.Generic.List<System.Drawing.Point>? TempData { get; set; }
    }
}