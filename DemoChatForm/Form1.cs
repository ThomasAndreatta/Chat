﻿#region Using
using DemoChatForm.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DemoChatForm.Class;
using DemoChatForm.Classi;
using DemoChatForm.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Generic;
#endregion


namespace DemoChatForm
{
    #region COSE DA FARE

    //da sistemare lo stealth, quando non appare in task bar non funzionano più gli hotkey:(

    

    //MANDA => string content = Convert.ToBase64String(File.ReadAllBytes(< percorsoFile >));
    //RICEVI => File.WriteAllBytes(<nome> +<estensione>, Convert.FromBase64String(<stringa>));
   
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
        private bool connesso = false;
        private string savingPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Nuova Cartella";
        public Listone listaUtenti = new Listone();
        private InvioUDP trasmettitore;
        private int CountPersone = 0;
        private riceviUDP ricevente;
       
        private Thread th1;
        private bool minimizato = false, stealth = false;
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
            trasmettitore = new InvioUDP();
            CheckForIllegalCrossThreadCalls = false;
            ricevente = new riceviUDP(9050);
            th1 = new Thread(Ascolta);
            th1.Start();
           
            Username FormUsername = new Username();
            FormUsername.ControlBox = false;
            if (FormUsername.ShowDialog() == DialogResult.OK)
            {
                CheckForIllegalCrossThreadCalls = false;
                trasmettitore.CambiaUsername(FormUsername.Username_Proprieties);
                if (trasmettitore.IpServer.Contains("127.0"))
                {            
                    aggiornaSitua(2);
                }
                else
                {                    
                    aggiornaSitua(1);
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
            trasmettitore.Invia($"                                   >>>{trasmettitore.Username.ToUpper()} ESCE DALLA CHAT<<<", 2);

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
                trasmettitore.Invia(msg,1);
                txtMsg.Text = "";
            }
        }
        private void AggiornaLbl()
        {
            lblPersoneOnline.Text = "" + listaUtenti.nomi.Count;
        }
        protected override void WndProc(ref Message m) //hotkey
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == nascondi)
            {
                if (stealth)
                {
                   this.ShowInTaskbar = false;
                }
                this.WindowState = FormWindowState.Minimized;
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == mostra)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
            base.WndProc(ref m);
        }
         /*private void spezzetta(string percorso, string ext, string nome)
        {
            string content = Convert.ToBase64String(File.ReadAllBytes(percorso));
            int numFramm = (System.Text.ASCIIEncoding.Unicode.GetByteCount(content) / 50000) + 1;//conta quante volte va diviso in file da 50kb e aggiunge 1 in caso sia 13,9 volte
            string msg = "";//messaggio che verrà mandato
            for (int i = 0; i <= numFramm; i++)
            {
                if (content.Length >= 24999)
                    msg = content.Substring(0, 24999); // ora il msg vale 50 kb
                else
                    msg = content.Substring(0, content.Length);

                if (i == numFramm)
                    msg = nome+"|"+ numFramm + "|" + i + "|" + msg + "|" + ext;
                else
                    msg = nome + "|" + numFramm + "|" + i + "|" + msg;


                trasmettitore.Invia(msg,4);//qui invio il mess

                if (content.Length >= 24999)
                    content = content.Remove(0, 24999);// ora il msg vale 50 kb
                else
                    content = content.Remove(0, content.Length);

            }
        }

       */
      

        #endregion

        #region Server
        private void Ascolta()
        {
            string msg;
            while (continua)
            {
                bool aggiunto = false;
                msg = ricevente.ricevi();

                if (this.WindowState == FormWindowState.Minimized && !msg.Contains(trasmettitore.Username.ToUpper() + " ENTRA NELLA CHAT"))
                {
                    this.Icon = MessNonLetto;
                    minimizato = !minimizato;
                }
                #region ingressi e dati
                if (msg.Contains("ENTRA NELLA CHAT"))
                {
                    listaUtenti.ip.Add(IPAddress.Parse(msg.Split('|')[0]));
                    listaUtenti.nomi.Add(msg.Split('|')[1]);
                    CountPersone++;
                    AggiornaLbl();
                    lstBoxMsg.Items.Add(msg.Split('|')[2]);
                    trasmettitore.Invia("struct§ù§|" + listaUtenti.nomi.Count + "|" + RitornaIp() + "|" + RitornaNomi(), 3);
                    aggiunto = true;
                }
                if (msg.Contains("ESCE DALLA CHAT"))//ip|nome|msg
                {
                    listaUtenti.ip.Remove(IPAddress.Parse(msg.Split('|')[0]));
                    listaUtenti.nomi.Remove(msg.Split('|')[1]);
                    lstBoxMsg.Items.Add(msg.Split('|')[2]);
                    CountPersone--;
                    AggiornaLbl();
                    aggiunto = true;
                }
                if (msg.Contains("HA INVIATO"))//ip|nome|msg
                {
                   
                    lstBoxMsg.Items.Add(msg);
                    
                   
                    aggiunto = true;
                }
                if (msg.Contains("struct???") && Convert.ToInt32(msg.Split('|')[1]) > CountPersone) //struct§ù§|numero|ip|username
                {
                    LeggiIp(msg.Split('|')[2], msg.Split('|')[3], Convert.ToInt32(msg.Split('|')[1]));

                    aggiunto = true;
                }
                if (aggiunto == false && msg.Contains("file")  && msg.Split('|')[1] != trasmettitore.Ip)//"file"|username|file in stringa|nome del file|estensione
                {
                    
                        if (MessageBox.Show($"{msg.Split('|')[1]} vuole inviarti {msg.Split('|')[3] }" +
                       $"\n Premere OK per scaricare", "File", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                        string path = savingPath + "\\" + msg.Split('|')[3];
                        File.WriteAllBytes(path, Convert.FromBase64String(msg.Split('|')[2]));
                    }
                   
                   
                    aggiunto = true;
                }
                
                else if (aggiunto == false && msg.Contains("justalk"))
                    {
                        lstBoxMsg.Items.Add(msg.Remove(msg.IndexOf('j'), "justalk".Length));
                    }
                   
                    #endregion
                
            }
        }
        private void LeggiIp(string listoneIp, string listoneNomi, int numero)
        {
            listoneIp.Remove(listoneIp.Length - 1);
            listaUtenti.ip.Clear();           
            listaUtenti.nomi.Clear();
            for (int i = 0; i < numero; i++)
            {
                listaUtenti.ip.Add(IPAddress.Parse(listoneIp.Split('-')[i]));
                listaUtenti.nomi.Add(listoneNomi.Split('-')[i]);
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
            return listaIp.Remove(listaIp.Length-1);
        }
        private string RitornaNomi()
        {
            string coso = "";
            foreach (var item in listaUtenti.nomi)
            {
                coso += item + "-";
            }
            return coso.Remove(coso.Length-1);
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
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog filedamandare = new OpenFileDialog();
            if (filedamandare.ShowDialog() == DialogResult.OK)
            {
                long length = new System.IO.FileInfo(filedamandare.FileName).Length;
                if (length <= 50000)
                {
                    string content = Convert.ToBase64String(File.ReadAllBytes(filedamandare.FileName));
                    trasmettitore.Invia(content+"|"+Path.GetFileName(filedamandare.FileName), 4);

                }
                else
                {
                    Point a = new Point(Pic_Allegato.Location.X, Pic_Allegato.Location.Y);
                    Notifica tmp = new Notifica("File troppo grande, dimensioni massime 50kb", 7, a);
                    tmp.Show();
                }                
            }
        }

       
        private void aggiornaSitua(int variabile)
        {
            if (variabile == 1)//connesso
            {
                lblPersoneOnline.Visible = true;
                listaUtenti.ip.Clear();
                listaUtenti.nomi.Clear();
                AggiornaLbl();
                lstBoxMsg.Items.Clear();
                trasmettitore.Invia($"                                   >>>({trasmettitore.Ip}){trasmettitore.Username.ToUpper()} ENTRA NELLA CHAT<<<", 2);
                connesso = true;
                lstBoxMsg.Visible = true;
                btnInvia.Visible = true;
                txtMsg.Visible = true;
                lblConnessione.Visible = false;
                Pic_Allegato.Visible = true;
                pic_persone.Visible = true;
               
            }
            else//non connesso
            {
                trasmettitore.Invia($"                                   >>>{trasmettitore.Username.ToUpper()} ESCE DALLA CHAT<<<", 2);

                lstBoxMsg.Items.Clear();
                connesso = false;
                lstBoxMsg.Visible = false;
                btnInvia.Visible = false;
                txtMsg.Visible = false;
                lblConnessione.Visible = true;
                Pic_Allegato.Visible = false;
                pic_persone.Visible = false;
                lblPersoneOnline.Visible = false;
            }
            
        }
        private void IconaUtenti_Click(object sender, EventArgs e)
        {
            
            FormUtenti formutenti = new FormUtenti(listaUtenti);
            formutenti.Show();
        }
        private void ImpostazioniIcon_Click(object sender, EventArgs e)
        {
            
            Impostazioni impo = new Impostazioni(stealth, savingPath);
            impo.ShowDialog();
            stealth =  impo.Lol;
            savingPath = impo.Path;
        }
        private void InfoIcon_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.Show();
        }
        #endregion   
    }
}