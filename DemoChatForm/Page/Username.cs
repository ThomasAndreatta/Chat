using System;
using System.Drawing;
using System.Windows.Forms;
using DemoChatForm.Class;

namespace DemoChatForm.Page
{
    public partial class Username : Form
    {
     
        public string username = "";
        public Username()
        {            
            InitializeComponent();
            
            this.TopMost = true;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            invio();            
        }

       

        private void invio()
        {
            if (textBox1.Text.Contains(" ") || textBox1.Text == "")
            {
                Point punto = new Point(textBox1.PointToScreen(Point.Empty).X + (this.Width-textBox1.Left)-10, textBox1.PointToScreen(Point.Empty).Y-15);//nella x aggiungere la dimensione del form, bisogna che la notifica parta appena fuori dal form
                string a = "Enter a valid user name(no blank text or space)";
                Notifica notifica = new Notifica(a, 5,punto);
                notifica.Show();
            }
            else
            {
                username = textBox1.Text;
                DialogResult = DialogResult.OK;
            }
                
            
        }
      
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                invio();
            }
        }

        private void Username_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }
    }
}
