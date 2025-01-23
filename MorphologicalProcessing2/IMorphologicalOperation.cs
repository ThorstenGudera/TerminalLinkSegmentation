using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphologicalProcessing2
{
    public interface IMorphologicalOperation : IDisposable
    {
        int[,]? Kernel { get; set; }
        Bitmap? KernelBmp { get; set; }
        bool Setup(int width, int height); 
        bool SetupEx(int width, int height);
        void ApplyGrayscale(Bitmap bmp);
        bool RotateDilationKernels {  get; set; }
        BackgroundWorker? BGW { get; set; }
    }
}
