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
    public partial class frmReducePathPoints : Form
    {
        public event IntegerEventHandler? FigureChanged;
        public event IntegerEventHandler? PathChanged;
        public delegate void IntegerEventHandler(object sender, IntegerEventArgs e);

        public frmReducePathPoints(int frm3MinDist, int frm3Epsilon, bool frm3ReducePoints, List<List<PointF>> curPath, List<List<List<PointF>>> pL)
        {
            InitializeComponent();

            this.CheckBox1.Checked = frm3ReducePoints;

            switch (frm3MinDist)
            {
                case 0:
                    {
                        this.ComboBox1.SelectedIndex = 0;
                        break;
                    }

                case 1:
                    {
                        this.ComboBox1.SelectedIndex = 1;
                        break;
                    }

                case  2:
                    {
                        this.ComboBox1.SelectedIndex = 2;
                        break;
                    }

                case 3:
                    {
                        this.ComboBox1.SelectedIndex = 3;
                        break;
                    }

                case 4:
                    {
                        this.ComboBox1.SelectedIndex = 4;
                        break;
                    }

                case 5:
                    {
                        this.ComboBox1.SelectedIndex = 5;
                        break;
                    }

                case 6:
                    {
                        this.ComboBox1.SelectedIndex = 6;
                        break;
                    }

                case 7:
                    {
                        this.ComboBox1.SelectedIndex = 7;
                        break;
                    }
            }

            switch (frm3Epsilon)
            {
                case 0:
                    {
                        this.ComboBox2.SelectedIndex = 0;
                        break;
                    }

                case 1:
                    {
                        this.ComboBox2.SelectedIndex = 1;
                        break;
                    }

                case 2:
                    {
                        this.ComboBox2.SelectedIndex = 2;
                        break;
                    }

                case 3:
                    {
                        this.ComboBox2.SelectedIndex = 3;
                        break;
                    }

                case 4:
                    {
                        this.ComboBox2.SelectedIndex = 4;
                        break;
                    }
            }

            if (pL != null)
            {
                this.ListBox1.SuspendLayout();
                this.ListBox1.Items.Clear();
                if (curPath != null)
                    this.ListBox1.Items.Add("Current Path");
                for (int i = 0; i <= pL.Count - 1; i++)
                    this.ListBox1.Items.Add("SavedPath_" + i.ToString());
                this.ListBox1.ResumeLayout();
                if (this.ListBox1.Items.Count > 0)
                    this.ListBox1.SelectedIndex = 0;
                this.ListBox1.Visible = true;

                this.Text = "Select path to edit";
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ListBox1.SelectedIndex > -1)
            {
                FigureChanged?.Invoke(this, new IntegerEventArgs() { Value = this.ListBox1.SelectedIndex });
                string? s = this.ListBox1.Items[0].ToString();
                if (s != null)
                PathChanged?.Invoke(this, new IntegerEventArgs() { Value = this.ListBox1.SelectedIndex, Index0IsCurrentPath = s.Contains("Current Path") });
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            ListBox1_SelectedIndexChanged(this.ListBox1, EventArgs.Empty);
        }
    }
}
