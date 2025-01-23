using ChainCodeFinder;
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
    public partial class frmShiftInwards : Form
    {
        private ShiftPathMode _spm;
        private ShiftFractionMode _sfm;

        public frmShiftInwards(ShiftPathMode spm, ShiftFractionMode sfm)
        {
            InitializeComponent();
            this._spm = spm;
            this._sfm = sfm;

            foreach (string z in Enum.GetNames(typeof(ShiftPathMode)))
                this.ComboBox1.Items.Add(z);

            this.ComboBox1.SelectedItem = this.ComboBox1.Items[0];
            for (int i = 0; i <= ComboBox1.Items.Count - 1; i++)
            {
                string? s = ComboBox1.Items[i]?.ToString();
                if (s != null)
                if ((ShiftPathMode)Enum.Parse(typeof(ShiftPathMode), s) == this._spm)
                {
                    ComboBox1.SelectedIndex = i;
                    break;
                }
            }

            foreach (string z in Enum.GetNames(typeof(ShiftFractionMode)))
                this.ComboBox2.Items.Add(z);
            this.ComboBox2.SelectedItem = this.ComboBox2.Items[0];
            for (int i = 0; i <= ComboBox2.Items.Count - 1; i++)
            {
                string? s = ComboBox2.Items[i]?.ToString();
                if (s != null)
                if ((ShiftFractionMode)Enum.Parse(typeof(ShiftFractionMode), s) == this._sfm)
                {
                    ComboBox2.SelectedIndex = i;
                    break;
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.NumericUpDown1.Value = 0;
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control ct in this.GroupBox1.Controls)
            {
                if (!ct.Equals(this.NumericUpDown1))
                    ct.Enabled = !this.CheckBox2.Checked;
            }
        }
    }
}
