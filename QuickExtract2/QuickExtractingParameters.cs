using System.Collections.Generic;
using System.Drawing;

namespace QuickExtract2
{
    internal class QuickExtractingParameters
    {
        public Bitmap? imgDataPic { get; set; }
        public Bitmap? bmpDataForValueComputation { get; set; }
        public bool doR { get; set; }
        public bool doG { get; set; }
        public bool doB { get; set; }
        public bool doScale { get; set; }
        public int neighbors { get; set; }
        public bool useCostMap { get; set; }
        public List<double[]>? Ramps { get; set; }
        public double valL { get; set; }
        public double valM { get; set; }
        public double valG { get; set; }
        public int laplTh { get; set; }
        public double edgeWeight { get; set; }
        public int dist { get; set; }
        public List<List<PointF>>? CurPath { get; set; }
        public List<PointF>? SeedPoints { get; set; }
        public double valP { get; set; }
        public double valI { get; set; }
        public double valO { get; set; }
        public bool MouseClicked { get; set; }
        public string? msg { get; set; }
        public List<PointF>? TempPath { get; set; }
        public int transX { get; set; }
        public int transY { get; set; }
        public int notifyEach { get; set; }
        public int amountX { get; set; }
        public int amountY { get; set; }
        public double valCl { get; set; }
        public double valCol { get; set; }

        //public void Dispose()
        //{
        //    if (this.imgDataPic != null)
        //        this.imgDataPic.Dispose();
        //    this.imgDataPic = null;
        //    if (this.bmpDataForValueComputation != null)
        //        this.bmpDataForValueComputation.Dispose();
        //    this.bmpDataForValueComputation = null;
        //}
    }
}