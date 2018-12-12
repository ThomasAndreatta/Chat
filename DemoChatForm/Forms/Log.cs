using System;
using System.Drawing;
using System.Windows.Forms;

using System.Net;

namespace DemoChatForm.Forms //in caso riuscissi ad implementare una chat privata
{
    public partial class Log : Form
    {
#region variabili
        private string username;
        private IPAddress ip;
        private int port;
        private string nome;
        private bool PortaOkay = true;
        private bool errore = false;
#endregion
        public Log()
        {
            InitializeComponent();
        }        
        private void Start_Click(object sender, EventArgs e)
        {
            if (Controllo_Start())
            {               
                port = Convert.ToInt32(txt_Porta.Text);               
                ip =IPAddress.Parse(txt_IP.Text);
                nome = textBox1.Text;
                DialogResult = DialogResult.OK;                
            }
            else
            {
                if (!errore)
                {
                    MessageBox.Show("Completare tutti i campi!");
                    if (errore)
                    {
                        errore = false;
                    }
                }
               
            }
        }
        private bool Controllo_Start()
        {
            if (PortaOkay && txt_IP.Text != "" && textBox1.Text!="" )
            {               
              
                    if (ControlloIP(txt_IP.Text))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Inserie un IP valido.");
                        errore = true;
                        return false;
                    }
               
            }
            else
            {
                return false;
            }
        }    
        private bool ControlloIP(string IPdaControllare)
        {
            IPAddress tmp_Address;
            bool thisGood = IPAddress.TryParse(IPdaControllare, out tmp_Address);
            try
            {
                string[] s = IPdaControllare.Split('.');
                if (s.Length != 4)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }            
        }                           
        private void Txt_Porta_TextChanged(object sender, EventArgs e)
        {
            if (txt_Porta.Text.Trim().Length > 0 && Int32.TryParse(txt_Porta.Text, out port)/*&& port >= 49152 && port <= 65535*/ )
            {
                txt_Porta.ForeColor = Color.Black;
                PortaOkay = true;
            }
            else
            {
                PortaOkay = false;
                txt_Porta.ForeColor = Color.Red;               
            }
        }

    }
}
