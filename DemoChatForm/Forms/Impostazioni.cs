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
        private string _path =  "";
        public Impostazioni(bool check,string path)
        {
            InitializeComponent();
            if (check)
            {
                checkBox1.Checked = true;
            }
            label1.Text = path;

        }

        public bool Lol { get => lol; }
        public string Path { get => _path;  }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            lol = !Lol;
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            _path = label1.Text;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                label1.Text = fbd.SelectedPath;
            }
        }
    }
}
