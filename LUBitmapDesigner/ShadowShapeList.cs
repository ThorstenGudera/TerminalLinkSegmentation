using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUBitmapDesigner
{
    public class ShadowShapeList : ShapeList
    {
        internal const int _SHADOWMAXSIZE = 3;

        public ShadowShapeList()
        { 
            
        }

        new internal void Add(BitmapShape b)
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

        public void SwapUpperShapes()
        {
            if(this.Count > 2)
                (this[1], this[2]) = (this[2], this[1]);
        }

    }
}
