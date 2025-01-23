using System.Collections.Generic;
using System.Drawing;
using System;

namespace QuickExtract2
{
    public class FigureChangedEventArgs : EventArgs
    {
        public List<PointF> FList { get; set; }
        public FigureChangedEventArgs(List<PointF> fL)
        {
            this.FList = fL;
        }
    }
}