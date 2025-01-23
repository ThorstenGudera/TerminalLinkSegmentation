using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUBitmapDesigner
{
    public class ShapeList
    {
        internal List<BitmapShape> Shapes { get; set; }
        public int Count
        {
            get { return Shapes.Count; }
        }

        public bool AllowAdding { get; set; }

        internal const int _MAXSIZE = 2;

        internal ShapeList()
        {
            Shapes = new List<BitmapShape>();
        }

        public BitmapShape this[int index]
        {
            get
            {
                return this.Shapes[index];
            }
            set
            {
                this.Shapes[index] = value;
            }
        }

        public void Add(BitmapShape b)
        {
            if (this.Shapes == null)
                this.Shapes = new List<BitmapShape>();

            if (this.Shapes.Count < ShadowShapeList._SHADOWMAXSIZE || this.AllowAdding)
            {
                this.Shapes.Add(b);
                this.AllowAdding = false;
            }
            else
                throw new Exception("This List already contains its maximum amount of Shapes");
        }

        public void Remove(BitmapShape? b)
        {
            if (this.Shapes == null)
                this.Shapes = new List<BitmapShape>();

            if (b != null && this.Shapes.Contains(b))
            {
                this.Shapes.Remove(b);
                b.Dispose();
                b = null;
            }
        }

        public void RemoveAt(int index)
        {
            BitmapShape? bitmapShape = this.Shapes[index];
            this.Shapes.RemoveAt(index);
            bitmapShape.Dispose();
            bitmapShape = null;
        }

        public void Clear()
        {
            for (int j = this.Shapes.Count - 1; j >= 0; j--)
            {
                BitmapShape? b = this.Shapes[j];
                b.Dispose();
                b = null;
            }

            this.Shapes.Clear();
        }

        public void ClearWithoutBG()
        {
            for (int j = this.Shapes.Count - 1; j > 0; j--)
            {
                BitmapShape? b = this.Shapes[j];
                this.Shapes.RemoveAt(j);
                b.Dispose();
                b = null;
            }
        }

        public BitmapShape? GetShapeById(int id)
        {
            BitmapShape? bOut = null;

            IEnumerable<BitmapShape> b = this.Shapes.Where(a => a.ID == id);
            if (b != null)
                bOut = b.First();

            return bOut;
        }

        public bool AddingAllowed(bool shadowMode)
        {
            if (shadowMode)
                return this.Count < ShadowShapeList._SHADOWMAXSIZE;
            else
                return this.Count < ShapeList._MAXSIZE;
        }
    }
}
