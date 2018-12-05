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
#endregion


namespace DemoChatForm
{
# region COSE DA FARE
    //all'arrivo di un messaggio con app chiusa cambia l'icona    
    //da mettere nelle impo la lingua(ita eng) e la stealth mode(non mostrarla nella taskbar) con un menu a tendina   
    //mettere "(ip)coso è entrato nella chat"
    
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
                tx.username = coso.username;
                rx = new riceviUDP(9050);
                th1 = new Thread(ascolta);
                th1.Start();
            }

            #region HOTKEY           
            RegisterHotKey(this.Handle, nascondi, 6, (int)Keys.H);
            RegisterHotKey(this.Handle, mostra, 6, (int)Keys.S);
            #endregion

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == nascondi)
            {
                this.Visible = false;   
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == mostra)
            {
                this.Visible = true;   
            }
            base.WndProc(ref m);
        }

       
        private void btnInvia_Click(object sender, EventArgs e)
        {
            if (txtMsg.Text != "")
            {
                tx.invia(txtMsg.Text);
                txtMsg.Clear();
            }
        }
        private void txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtMsg.Text != "")
                {
                    tx.invia(txtMsg.Text);
                    txtMsg.Clear();
                }

            }
        }  
        private void form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            continua = false;
            th1.Abort();
        }

       
        #endregion

        #region Server
        private void ascolta()
        {
            string msg;
            while (continua)
            {
       
                msg = rx.ricevi();
                if (msg.Contains( "<clear>"))
                {
                    lstBoxMsg.Items.Clear();
                }
                else
                    lstBoxMsg.Items.Add(msg);
            }
        }
        #endregion

        #region Icone
        private void cambiaUsernameIcon_Click(object sender, EventArgs e)
        {
            Username newUsername = new Username();
            if (newUsername.ShowDialog() == DialogResult.OK)
            {
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
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Point a = new Point(10, 20);
            //Notifica coso = new Notifica("banano", 10,Point a);
            //coso.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void infoIcon_Click(object sender, EventArgs e)
        {
          
            Info info = new Info();
            info.Show();
        }
        #endregion   


    }
}