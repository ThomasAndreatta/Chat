using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoChatForm.Page
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.TopMost = true;
            if (true)//inglese
            {
                label1.Text = "When you're in Stealth mode the application won't appear on your taskbar.\n" +
                    "For Hide the chat press: \"Ctrl+Alt+H\", to make her reappear press \"Ctrl+Alt+S\"\n" +
                    "For clear the chat type \"<clear>\" and press enter.\n" +
                    "";
            }
            else//ita
            {

            }
            
        }
    }
}
