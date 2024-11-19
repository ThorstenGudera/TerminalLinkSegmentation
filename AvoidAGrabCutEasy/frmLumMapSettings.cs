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
    public partial class frmLumMapSettings : Form
    {
        public frmLumMapSettings()
        {
            InitializeComponent();
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            this.numThMultiplier.Enabled = this.label6.Enabled = !cbAuto.Checked;
        }
    }
}
