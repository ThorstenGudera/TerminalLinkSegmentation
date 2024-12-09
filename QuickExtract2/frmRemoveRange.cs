using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickExtract2
{
    public partial class frmRemoveRange : Form
    {
        private int _indx;
        private frmEditPath? _frm;

        public int StartIndex { get; internal set; }
        public int EndIndex { get; internal set; }

        public event EventHandler<IntegerEventArgs4>? NumsChanged;
        public event EventHandler<IntegerEventArgs4>? RemovePoints;

        public frmRemoveRange()
        {
            InitializeComponent();
        }

        public frmRemoveRange(frmEditPath frm, int indx, int selctedStPtIndex, int selctedEPtIndex)
        {
            InitializeComponent();

            this._indx = indx;
            this._frm = frm;
            this.StartIndex = selctedStPtIndex;
            this.EndIndex = selctedEPtIndex;
            this.numStIndx.Value = (decimal)this.StartIndex;
            this.numEIndx.Value = (decimal)this.EndIndex;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.StartIndex = (int)this.numStIndx.Value;
            NumsChanged?.Invoke(this, new IntegerEventArgs4() { Start = this.StartIndex, End = this.EndIndex });
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            this.EndIndex = (int)this.numEIndx.Value;
            NumsChanged?.Invoke(this, new IntegerEventArgs4() { Start = this.StartIndex, End = this.EndIndex });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RemovePoints?.Invoke(this, new IntegerEventArgs4() { Start = this.StartIndex, End = this.EndIndex });
        }
    }
}
