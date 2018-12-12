using System;
using System.Drawing;
using System.Windows.Forms;
using DemoChatForm.Class;

namespace DemoChatForm.Forms
{
    public partial class Username : Form
    {
     
        private string username = "";
        public Username()
        {            
            InitializeComponent();
            
            this.TopMost = true;            
        }
        public string Username_Proprieties { get => username; set => username = value; }
        private void Button1_Click(object sender, EventArgs e)
        {
            Invio();            
        }      
        private void Invio()
        {
            if (txt_Username.Text.Contains(" ") || txt_Username.Text == "")
            {
                Point punto = new Point(txt_Username.PointToScreen(Point.Empty).X + (this.Width-txt_Username.Left)-10, txt_Username.PointToScreen(Point.Empty).Y-15);//nella x aggiungere la dimensione del form, bisogna che la notifica parta appena fuori dal form
                string a = "Enter a valid user name(no blank text or space)";
                Notifica notifica = new Notifica(a, 5,punto);
                notifica.Show();
            }
            else
            {
                Username_Proprieties = txt_Username.Text;
                DialogResult = DialogResult.OK;
            }
                
            
        }      
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Invio();
            }
        }
        private void Username_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txt_Username;
        }
    }
}
