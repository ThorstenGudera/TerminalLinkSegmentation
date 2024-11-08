using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System;

namespace AvoidAGrabCutEasy
{
    [Serializable]
    public class SavedScribbles : IDisposable
    {
        public int[]? BGSizes { get; set; } = null;
        public Point[][][]? BGPoints { get; set; }

        public int[]? FGSizes { get; set; } = null;
        public Point[][][]? FGPoints { get; set; }

        public string? Bmp { get; set; }
        public int[]? UknwnSizes { get; set; }
        public Point[][][]? UknwnPoints { get; set; }
        public List<Tuple<int, int, int, bool, List<List<Point>>>>? ScribbleSequence { get; set; }
        public Size? BaseSize { get;  set; }

        public SavedScribbles()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_BGSizes", this.BGSizes);
            info.AddValue("_BGPoints", this.BGPoints);
            info.AddValue("_FGSizes", this.FGSizes);
            info.AddValue("_FGPoints", this.FGPoints);
            info.AddValue("_UnknwnSizes", this.UknwnSizes);
            info.AddValue("_UnknwnPoints", this.UknwnPoints);
            if (this.Bmp != null)
                info.AddValue("_Bmp", this.Bmp);
        }

        public Dictionary<int, Dictionary<int, List<List<Point>>>> ToDictionary()
        {
            Dictionary<int, Dictionary<int, List<List<Point>>>> res = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

            if (this.BGSizes != null)
            {
                res.Add(0, new Dictionary<int, List<List<Point>>>());
                for (int i = 0; i < this.BGPoints?.Length; i++)
                {
                    res[0].Add(this.BGSizes[i], new List<List<Point>>());

                    for (int j = 0; j < this.BGPoints[i].Length; j++)
                        res[0][this.BGSizes[i]].Add(this.BGPoints[i][j].ToList());
                }
            }

            if (this.FGSizes != null && this.FGPoints != null)
            {
                res.Add(1, new Dictionary<int, List<List<Point>>>());
                for (int i = 0; i < this.FGPoints.Length; i++)
                {
                    res[1].Add(this.FGSizes[i], new List<List<Point>>());

                    for (int j = 0; j < this.FGPoints[i].Length; j++)
                        res[1][this.FGSizes[i]].Add(this.FGPoints[i][j].ToList());
                }
            }

            if (this.UknwnSizes != null && this.UknwnPoints != null)
            {
                res.Add(3, new Dictionary<int, List<List<Point>>>());
                for (int i = 0; i < this.UknwnPoints.Length; i++)
                {
                    res[3].Add(this.UknwnSizes[i], new List<List<Point>>());

                    for (int j = 0; j < this.UknwnPoints[i].Length; j++)
                        res[3][this.UknwnSizes[i]].Add(this.UknwnPoints[i][j].ToList());
                }
            }

            return res;
        }

        public void Dispose()
        {
            if (this.Bmp != null)
                this.Bmp = null;

            this.BGSizes = null;
            this.FGSizes = null;
            this.BGPoints = null;
            this.FGPoints = null;
            this.UknwnSizes = null;
            this.UknwnPoints = null;
        }
    }
}