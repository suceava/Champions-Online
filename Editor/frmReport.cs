using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CharacterBuilder.ChampionsOnline.Editor
{
    public partial class frmReport : Form
    {
        public frmReport(string reportString)
        {
            InitializeComponent();
            txtReport.Text = reportString;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
