using System;
using System.IO;
using System.Windows.Forms;
using DemoChatForm.Classi;
namespace DemoChatForm.Forms
{
    public partial class FormUtenti : Form
    {
        public FormUtenti(Listone lista)
        {
            InitializeComponent();
            for (int i = 0; i < lista.ip.Count; i++)
            {             
                dataGridView1.Rows.Add(lista.nomi[i], lista.ip[i].ToString());
            }
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
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
