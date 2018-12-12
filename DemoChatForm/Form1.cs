#region Using
using DemoChatForm.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DemoChatForm.Class;
using DemoChatForm.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Generic;
#endregion


namespace DemoChatForm
{
    #region COSE DA FARE

    //inserire la steath dalle impo assieme alla lingua(cercare di pescare la roba da un file e non da if dai
    //ogni volta che qualcuno entra ed esce c'è da aggiornare lo struct listautenti e mostrarlo al click dell'immagine in un piccolo form con una tabella indirizzi e username.
    #endregion
    public partial class Form1 : Form
    {
        #region HotKey
        //per registrare una hot key:  RegisterHotKey(this.Handle, [id numerico con cui verrà ricoonosciuta da wndproc], combotasti, (int)Keys.TASTO);
        //tasti =  Alt = 1, Ctrl = 2, Shift = 4, Win = 8 (per metterne più assieme basta sommarli
        //che fa cose è il metodo wndproc
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region variabili 
        private struct Indirizzi
        {
            public List<string> nomi;
            public List<IPAddress> ip;
        }
        private Indirizzi listaUtenti = new Indirizzi();
        private InvioUDP trasmettitore;
        private int CountPersone = 0;
        private riceviUDP ricevente;
        private Thread th1;
        private bool minimizato = false;
        private bool continua = true, Qr = false;
        private Icon MessNonLetto = new Icon(Resources.Notifica2, new Size(51, 50));
        private Icon Logo = new Icon(Resources.logo, new Size(50, 50));
        private QrCode code;
        const int mostra = 2, nascondi = 1;
        private KeyboardHook hook = new KeyboardHook();
        #endregion

        #region Eventi Del Form
        public Form1()
        {
            InitializeComponent();

            listaUtenti.nomi = new List<string>();
            listaUtenti.ip = new List<IPAddress>();
            Username FormUsername = new Username();
            FormUsername.ControlBox = false;
            if (FormUsername.ShowDialog() == DialogResult.OK)
            {
                CheckForIllegalCrossThreadCalls = false;
                trasmettitore = new InvioUDP();
                if (trasmettitore.IpServer.Contains("127.0"))
                {
                    lstBoxMsg.Visible = false;
                    btnInvia.Visible = false;
                    txtMsg.Visible = false;
                    lblConnessione.Visible = true;
                }
                else
                {
                    trasmettitore.CambiaUsername(FormUsername.Username_Proprieties);
                    ricevente = new riceviUDP(9050);
                    th1 = new Thread(Ascolta);
                    th1.Start();
                    trasmettitore.Invia($"                                   >>>({trasmettitore.Ip}){trasmettitore.Username.ToUpper()} ENTRA NELLA CHAT<<<", 1);
                }
            }

            #region HOTKEY           
            RegisterHotKey(this.Handle, nascondi, 6, (int)Keys.Q);
            RegisterHotKey(this.Handle, mostra, 6, (int)Keys.W);
            #endregion

        }
     
        private void BtnInvia_Click(object sender, EventArgs e)
        {
            if (txtMsg.Text != "")
            {
                Invio(txtMsg.Text);
            }
        }       
        private void TxtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //tasto invio
            {
                if (txtMsg.Text != "")
                {
                    Invio(txtMsg.Text);
                }

            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            trasmettitore.Invia($"                                   >>>{trasmettitore.Username.ToUpper()} ESCE DALLA CHAT<<<", 1);

            try
            {
                th1.Abort();
                continua = false;
            }
            catch (Exception)
            { }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (minimizato)
            {
                this.Icon = Logo;
                minimizato = !minimizato;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }
        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (txtMsg.Text != "")
                {
                    Invio(txtMsg.Text);

                }
            }
            else if (e.KeyValue != 46 && e.KeyValue != 8)
            {
                if (!txtMsg.Focused)
                {
                    txtMsg.Focus();
                    txtMsg.Text += Convert.ToChar(e.KeyValue);
                    txtMsg.SelectionStart = txtMsg.Text.Length;
                }

            }
        }
        private void TxtMsg_TextChanged(object sender, EventArgs e)//rubato da stackoverflow
        {
            if (txtMsg.Text.Length <= 0) return;
            string s = txtMsg.Text.Substring(0, 1);
            if (s != s.ToUpper())
            {
                int curSelStart = txtMsg.SelectionStart;
                int curSelLength = txtMsg.SelectionLength;
                txtMsg.SelectionStart = 0;
                txtMsg.SelectionLength = 1;
                txtMsg.SelectedText = s.ToUpper();
                txtMsg.SelectionStart = curSelStart;
                txtMsg.SelectionLength = curSelLength;
            }
        }        
        #endregion

        #region Logiche
        private void Invio(string msg)
        {
            if (msg.Contains("<clear>"))
            {
                txtMsg.Text = "";
                lstBoxMsg.Items.Clear();
            }
            else
            {
                trasmettitore.Invia(msg);
                txtMsg.Text = "";
            }
        }
        private void AggiornaLbl()
        {
            lblPersone.Text = "" + listaUtenti.nomi.Count;
        }
        protected override void WndProc(ref Message m) //hotkey
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == nascondi)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == mostra)
            {
                this.WindowState = FormWindowState.Normal;
            }
            base.WndProc(ref m);
        }

        #endregion

        #region Server
        private void Ascolta()
        {
            string msg;
            while (continua)
            {
                msg = ricevente.ricevi();

                if (this.WindowState == FormWindowState.Minimized && !msg.Contains(trasmettitore.Username.ToUpper() + " ENTRA NELLA CHAT"))
                {
                    this.Icon = MessNonLetto;
                    minimizato = !minimizato;
                }

                #region ingressi e dati
                if (msg.Contains("ENTRA NELLA CHAT"))//
                {
                    listaUtenti.ip.Add(IPAddress.Parse(msg.Split('|')[0]));
                    listaUtenti.nomi.Add(msg.Split('|')[1]);
                    CountPersone++;
                    AggiornaLbl();
                    lstBoxMsg.Items.Add(msg.Split('|')[2]);
                    trasmettitore.Invia("struct§ù§|" + listaUtenti.nomi.Count + "|" + RitornaIp() + "|" + RitornaNomi());
                }
                if (msg.Contains("ESCE DALLA CHAT"))//ip|nome|msg
                {
                    listaUtenti.ip.Remove(IPAddress.Parse(msg.Split('|')[0]));
                    listaUtenti.nomi.Remove(msg.Split('|')[1]);
                    lstBoxMsg.Items.Add(msg.Split('|')[2]);
                    CountPersone--;
                    AggiornaLbl();
                }
                if (msg.Contains("struct§ù§") && Convert.ToInt32(msg.Split('|')[1]) > CountPersone) //struct§ù§|numero|ip|username
                {
                    LeggiIp(msg.Split('|')[2], msg.Split('|')[3], Convert.ToInt32(msg.Split('|')[1]));
                }
                #endregion
            }
        }
        private void LeggiIp(string listoneIp, string listoneNomi, int numero)
        {
            listaUtenti.ip.RemoveAll((x) => x != null);
            listaUtenti.nomi.RemoveAll((x) => x != null);
            for (int i = 0; i <= numero; i++)
            {
                listaUtenti.ip.Add(IPAddress.Parse(listoneIp.Split('-')[i]));
                listaUtenti.nomi.Add(listoneIp.Split('-')[i]);
            }
            AggiornaLbl();
        }
        private string RitornaIp()
        {
            string listaIp = "";
            foreach (var item in listaUtenti.ip)
            {
                listaIp += item + "-";
            }
            return listaIp;
        }
        private string RitornaNomi()
        {
            string coso = "";
            foreach (var item in listaUtenti.nomi)
            {
                coso += item + "-";
            }
            return coso;
        }      
        #endregion
     
        #region Icone
        private void CambiaUsernameIcon_Click(object sender, EventArgs e)
        {
            Username newUsername = new Username();
            if (newUsername.ShowDialog() == DialogResult.OK)
            {
                trasmettitore.Invia($"                                   >>>{trasmettitore.Username.ToUpper()} HA CAMBIATO NOME IN {newUsername.Username_Proprieties}<<<", 1);
                trasmettitore.CambiaUsername(newUsername.Username_Proprieties);
            }
        }
        private void QrIcon_MousoOver(object sender, EventArgs e)
        {
            Qr = true;
            code = new QrCode(System.Windows.Forms.Cursor.Position.X + 10, System.Windows.Forms.Cursor.Position.Y + 10);
            code.Show();
        }
        private void QrIcon_MouseLeave(object sender, EventArgs e)
        {
            if (Qr)
            {
                code.Close();
                Qr = !Qr;
            }

        }
        private void SaveIcon_Click(object sender, EventArgs e)
        {
            if (lstBoxMsg.Items.Count == 0)
            {
                DialogResult dialogResult = MessageBox.Show("The chat is empty, save anyway?", "Saving blank chat", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    Salva();
            }
            else
                Salva();


        }
        private void Salva()
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "Chat" + " " + DateTime.Now.ToShortDateString().Replace('/', '-');
            save.DefaultExt = "txt";
            save.Title = "Save Chat";

            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (save.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter bw = new StreamWriter(File.Create(save.FileName)))
                {
                    foreach (var item in lstBoxMsg.Items)
                    {
                        bw.WriteLine(item);
                    }
                    bw.Close();
                }
                MessageBox.Show("Chat saved as: " + save.FileName);
            }
        }
        private void ListaUtenti_Icon_Click(object sender, EventArgs e)
        {
            MessageBox.Show(RitornaIp());
        }
        private void ImpostazioniIcon_Click(object sender, EventArgs e)
        {
            Point a = new Point(System.Windows.Forms.Cursor.Position.X + 10, System.Windows.Forms.Cursor.Position.Y - 40);
            Notifica not = new Notifica("Ti ho detto che non funziono.", 5, a);
            not.Show();
        }
        private void InfoIcon_Click(object sender, EventArgs e)
        {

            Info info = new Info();
            info.Show();
        }
        #endregion   
    }
}