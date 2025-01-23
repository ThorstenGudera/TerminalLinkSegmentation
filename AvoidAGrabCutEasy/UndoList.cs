using MorphologicalProcessing2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvoidAGrabCutEasy
{
    public class UndoList : IEnumerable<BitmapInfo>
    {
        private List<BitmapInfo> _list = new List<BitmapInfo>();
        public List<BitmapInfo> BitmapInfos
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }

        public BitmapInfo this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        private int _fposition = 0;
        public int CurrentPosition
        {
            get
            {
                return _fposition;
            }
            set
            {
                _fposition = value;
            }
        }

        public void Add(BitmapInfo c)
        {
            _list.Add(c);
            _fposition++;
        }

        public void RemoveAt(int indx)
        {
            _fposition--;
            this._list.RemoveAt(indx);
        }

        public void RemoveRange(int indx, int cnt)
        {
            if (_list == null || _list.Count <= 0)
                return;

            try
            {
                for (int i = indx + cnt - 1; i >= indx; i--)
                    if (this[i].Bmp != null)
                        this[i].Bmp?.Dispose();
                _list.RemoveRange(indx, cnt);
            }
            catch
            {
            }
        }

        public void Clear()
        {
            _list.Clear();
            _fposition = 0;
        }

        public void Undo()
        {
            if (CurrentPosition > 0)
                CurrentPosition -= 1;
        }

        public void Redo()
        {
            if (CurrentPosition < this.Count)
                CurrentPosition += 1;
        }

        public IEnumerator<BitmapInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
