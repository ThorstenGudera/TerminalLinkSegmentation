﻿using System;
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
    public partial class frmInfo : Form
    {
            public frmInfo()
            {
                InitializeComponent();
            }

            public frmInfo(string infoText)
            {
                InitializeComponent();
                this.Label1.Text = infoText;
            }
    }
}