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
    public partial class frmSelectCrop : Form
    {
        public event EventHandler? PathsChanged;

        public frmSelectCrop(List<List<List<PointF>>>? pathList)
        {
            InitializeComponent();
            if (pathList != null && pathList.Count > 0)
            {
                this.CheckedListBox1.SuspendLayout();
                this.CheckedListBox1.BeginUpdate();
                for (int i = 0; i <= pathList.Count - 1; i++)
                    this.CheckedListBox1.Items.Add("SavedPath_" + i.ToString(), false);
                this.CheckedListBox1.EndUpdate();
                this.CheckedListBox1.ResumeLayout();
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            PathsChanged?.Invoke(this, new EventArgs());
        }

        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PathsChanged?.Invoke(this, new EventArgs());
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            CheckedListBox1_SelectedIndexChanged(this.CheckedListBox1, EventArgs.Empty);
        }
    }
}
