using System;
using System.Drawing;

namespace MorphologicalProcessing2.Algorithms
{
    internal interface IExtendedMorphOP : IDisposable
    {
        Bitmap? MaskBmp { get; set; }
        int Tolerance { get; set; }
    }
}