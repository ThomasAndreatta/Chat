#region Using
using DemoChatForm.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DemoChatForm.Class;
using DemoChatForm.Page;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Net;
#endregion


namespace DemoChatForm
{
# region COSE DA FARE
   
   //inserire la steath dalle impo assieme alla lingua(cercare di pescare la roba da un file e non da if dai
        
#endregion
    public partial class Form1 : Form
    {
        #region HotKey
        //per registrare una hot key:  RegisterHotKey(this.Handle, [id numerico con cui verrà ricoonosciuta da wndproc], combotasti, (int)Keys.TASTO);
        //tasti =  Alt = 1, Ctrl = 2, Shift = 4, Win = 8 (per metterne più assieme basta sommarli
        //che fa cose è il metodo wndproc
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion
        
        #region variabili  
        InvioUDP tx;
        riceviUDP rx;
        private Thread th1;
        private bool minimizato = false;
        private bool continua = true, Qr = false;
        private Icon MessNonLetto = new Icon(Resources.Notifica2, new Size(50, 50));
        private Icon Logo = new Icon(Resources.logo, new Size(50, 50));
        private QrCode code;
        const int mostra = 2, nascondi = 1;
        private KeyboardHook hook = new KeyboardHook();
        
        #endregion

        #region Eventi Del Form
        public Form1()
        {           
            InitializeComponent();            
            Username coso = new Username();
            coso.ControlBox = false;
            if (coso.ShowDialog() == DialogResult.OK)
            {
                CheckForIllegalCrossThreadCalls = false;
                tx = new InvioUDP();
                if (tx.ip.Contains("127.0"))
                {
                    lstBoxMsg.Visible = false;
                    btnInvia.Visible = false;
                    txtMsg.Visible = false;
                    lblConnessione.Visible = true;
                }
                else
                {
                    tx.username = coso.username;
                    rx = new riceviUDP(9050);
                    th1 = new Thread(ascolta);
                    th1.Start();
                    tx.invia($"                                   >>>({tx.ip}){tx.username.ToUpper()} ENTRA NELLA CHAT<<<", 1);
                }               
            }
            
            #region HOTKEY           
            RegisterHotKey(this.Handle, nascondi, 6, (int)Keys.Q);
            RegisterHotKey(this.Handle, mostra, 6, (int)Keys.W);
            #endregion

        }        
        protected override void WndProc(ref Message m)
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
        private void btnInvia_Click(object sender, EventArgs e)
        {
            if (txtMsg.Text != "")
            {
                invio(txtMsg.Text);
            }
        }
        private void invio(string msg)
        {
            if (msg.Contains("<clear>"))
            {
                txtMsg.Text = "";
                lstBoxMsg.Items.Clear();
            }
            else
            {
                tx.invia(msg);
                txtMsg.Text = "";
            }
        }
        private void txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtMsg.Text != "")
                {
                    invio(txtMsg.Text);
                }

            }
        }  
        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
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
                    invio(txtMsg.Text);

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
        private void txtMsg_TextChanged(object sender, EventArgs e)
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

        #region Server
        private void ascolta()
        {
            string msg;
            while (continua)
            {
                msg = rx.ricevi();                
                lstBoxMsg.Items.Add(msg);
                if (this.WindowState == FormWindowState.Minimized && !msg.Contains(tx.username.ToUpper()+" ENTRA NELLA CHAT"))
                {
                    this.Icon = MessNonLetto;
                    minimizato = !minimizato;
                }
                
            }
        }
        #endregion

        #region Icone
        private void cambiaUsernameIcon_Click(object sender, EventArgs e)
        {
            Username newUsername = new Username();
            if (newUsername.ShowDialog() == DialogResult.OK)
            {                
                tx.invia($"                                   >>>{tx.username.ToUpper()} HA CAMBIATO NOME IN {newUsername.username}<<<", 1);
                tx.CambiaUsername(newUsername.username);
            }
        }
        private void qrIcon_MousoOver(object sender, EventArgs e)
        {
            Qr = true;
            code = new QrCode(System.Windows.Forms.Cursor.Position.X + 10, System.Windows.Forms.Cursor.Position.Y+10);
            code.Show();
        }
        private void qrIcon_MouseLeave(object sender, EventArgs e)
        {
            if (Qr)
            {
                code.Close();
                Qr = !Qr;
            }

        }
        private void saveIcon_Click(object sender, EventArgs e)
        {
            if (lstBoxMsg.Items.Count == 0)
            {
                DialogResult dialogResult = MessageBox.Show("The chat is empty, save anyway?", "Saving blank chat", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    salva();
            }
            else
                salva();


        }
        private void salva()
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
        private void impostazioniIcon_Click(object sender, EventArgs e)
        {
            Point a = new Point(System.Windows.Forms.Cursor.Position.X + 10, System.Windows.Forms.Cursor.Position.Y -40);
           Notifica not = new Notifica("Ti ho detto che non funziono.", 5, a);
            not.Show();
        }     
        private void infoIcon_Click(object sender, EventArgs e)
        {
          
            Info info = new Info();
            info.Show();
        }
        #endregion   
    }
}