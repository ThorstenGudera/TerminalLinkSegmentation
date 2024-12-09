using System;

namespace QuickExtract2
{
    public class IntegerEventArgs : EventArgs
    {
        public int Value { get; set; }
        public bool Index0IsCurrentPath { get; set; }
    }
}