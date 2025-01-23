using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickExtract2
{
    public partial class frmSelFig : Form
    {
        private GraphicsPath _gP;
        private List<List<PointF>>? _fList;

        public event EventHandler<FigureChangedEventArgs>? FigureChanged;

        public frmSelFig(GraphicsPath gp)
        {
            InitializeComponent();

            this._gP = gp;
            LoadListBox();
        }

        private void LoadListBox()
        {
            if (this._gP != null && this._gP.PointCount > 1)
            {
                this._fList = new List<List<PointF>>();
                for (int i = 0; i <= this._gP.PathTypes.Length - 1; i++)
                {
                    int j = i;
                    List<PointF> fL = new List<PointF>();
                    // While (j < Me._gP.PointCount) AndAlso (Me._gP.PathTypes(j) <> 129 AndAlso (Me._gP.PathTypes(j) <> 0 OrElse j = 0))
                    while ((j < this._gP.PointCount) && (this._gP.PathTypes[j] != 0 || j == 0))
                        j += 1;
                    fL.AddRange(this._gP.PathPoints.Skip(i).Take(j - i));
                    i = j;
                    this._fList.Add(fL);
                    this.ListBox1.Items.Add("Figure_" + this.ListBox1.Items.Count.ToString());
                }
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1 && this._fList != null && this._fList.Count > this.ListBox1.SelectedIndex)
            {
                List<PointF> fL = this._fList[this.ListBox1.SelectedIndex];
                FigureChanged?.Invoke(this, new FigureChangedEventArgs(fL));
            }
        }
    }
}
