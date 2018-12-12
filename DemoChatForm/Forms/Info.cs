using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoChatForm.Forms
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
                label1.Text = "Per nascondere la chat premere: \"Ctrl+Shift+Q\", per farla riapparire premere: \"Ctrl+Shift+W\"\n" +
                    "Per ripulire la chat inserire \"<clear>\" e inviare.\n" +
                    "";
            }
            else//ita
            {

            }
            
        }
    }
}
