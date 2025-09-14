using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlineOperations
{
    public partial class frmSetBGColor : Form
    {
        private static int[] CustomColors = new int[] { };

        public frmSetBGColor(Color c)
        {
            InitializeComponent();   
            this.label1.BackColor = c;
        }

        private void btnSelCol_Click(object sender, EventArgs e)
        {
            this.colorDialog1.CustomColors = CustomColors;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.label1.BackColor = this.colorDialog1.Color;
                CustomColors = this.colorDialog1.CustomColors;
            }
        }
    }
}
