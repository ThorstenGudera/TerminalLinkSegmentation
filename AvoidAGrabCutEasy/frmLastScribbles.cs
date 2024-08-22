using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmLastScribbles : Form
    {
        private Dictionary<int, Dictionary<int, List<List<Point>>>>? _scribbles;
        public frmLastScribbles(Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles)
        {
            InitializeComponent();
            this._scribbles = scribbles;
        }

        private void frmLastScribbles_Load(object sender, EventArgs e)
        {
            foreach (string z in System.Enum.GetNames(typeof(ScribblesType)))
                this.cmbScribblesType.Items.Add(z.ToString());

            this.cmbScribblesType.SelectedIndex = 0;
        }

        private void cnbScribblesType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                if (this._scribbles.ContainsKey(fg))
                {
                    Dictionary<int, List<List<Point>>> z = this._scribbles[fg];
                    if (z.Keys.Count > 0)
                    {
                        this.listBox1.Items.Clear();
                        foreach (int i in z.Keys)
                        {
                            this.listBox1.Items.Add(i.ToString());
                        }
                    }
                }

                if (this.listBox1.Items.Count > 0)
                    this.listBox1.SelectedIndex = 0;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._scribbles != null && this.cmbScribblesType.SelectedIndex > -1)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                if (this._scribbles.ContainsKey(fg))
                {
                    Dictionary<int, List<List<Point>>> z = this._scribbles[fg];
                    if (z.Keys.Count > 0)
                    {
                        this.listBox2.Items.Clear();

                        int j = -1;

                        if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out j))
                        {
                            List<List<Point>> wh = z[j];
                            for (int i = 0; i < wh.Count; i++)
                                this.listBox2.Items.Add(wh[i].Count.ToString());
                        }
                    }
                }

                if (this.listBox2.Items.Count > 0)
                    this.listBox2.SelectedIndex = 0;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this._scribbles != null && MessageBox.Show("Delete this item from the scribbles-collection?", "",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                int fg = this.cmbScribblesType.SelectedIndex == 0 ? 0 : this.cmbScribblesType.SelectedIndex == 1 ? 1 : 3;
                if (this._scribbles.ContainsKey(fg))
                {
                    int j = -1;

                    if (Int32.TryParse(this.listBox1.SelectedItem?.ToString(), out j))
                    {
                        int wh = j;

                        List<List<Point>> list = this._scribbles[fg][wh];
                        int j2 = -1;

                        if (Int32.TryParse(this.listBox2.SelectedItem?.ToString(), out j2))
                            if (list.Count > this.listBox2.SelectedIndex && list[this.listBox2.SelectedIndex].Count == j2)
                            {
                                this.listBox2.SuspendLayout();
                                this._scribbles[fg][wh].RemoveAt(this.listBox2.SelectedIndex);
                                listBox1_SelectedIndexChanged(listBox1, new EventArgs());
                                this.listBox2.ResumeLayout();
                            }
                    }
                }
            }
        }
    }
}
