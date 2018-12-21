using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DemoChatForm.Classi;
namespace DemoChatForm.Forms
{
    public partial class FormUtenti : Form
    {
        private List<string> nominativi;
        private List<string> ip;
       
        public FormUtenti(Listone lista)
        {
            nominativi = lista.nomi;
            ip = lista.IpList();
            InitializeComponent();
            for (int i = 0; i < ip.Count; i++)
            {             
                dataGridView1.Rows.Add(nominativi[i], ip[i]);
            }
            dataGridView1.Height = DgvHeight();           
           
            //this.ClientSize = new System.Drawing.Size(dataGridView1.Size.Width, textBox1.Top + textBox1.Size.Height + 10);

        }
        private int DgvHeight()
        {
            int sum = this.dataGridView1.ColumnHeadersHeight;

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
                sum += row.Height ; // I dont think the height property includes the cell border size, so + 1

            return sum-3;
        }
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "Ip" + " " + DateTime.Now.ToShortDateString().Replace('/', '-');
            save.DefaultExt = "txt";
            save.Title = "Save Chat";

            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (save.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter bw = new StreamWriter(File.Create(save.FileName)))
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        bw.WriteLine(dataGridView1[0, i].Value.ToString() + " - " + dataGridView1[1, i].Value.ToString());
                    }
                    bw.Close();
                }
                MessageBox.Show("IP list saved as: " + save.FileName);
            }
        }

     
    }
}
