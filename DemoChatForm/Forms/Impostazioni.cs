using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoChatForm.Forms
{
    public partial class Impostazioni : Form
    {
        private bool lol = false;
        public Impostazioni(bool check)
        {
            InitializeComponent();
            if (check)
            {
                checkBox1.Checked = true;
            }
        }

        public bool Lol { get => lol; }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            lol = !Lol;
        }
    }
}
