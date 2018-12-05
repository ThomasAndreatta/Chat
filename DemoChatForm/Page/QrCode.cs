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
    public partial class QrCode : Form
    {

        private int desiredStartLocationX;
        private int desiredStartLocationY;

        public QrCode(int x, int y)
               
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.desiredStartLocationX = x;
            this.desiredStartLocationY = y;

            Load += new EventHandler(QrCode_Load_1);
        }

        private void QrCode_Load_1(object sender, EventArgs e)
        {
            this.SetDesktopLocation(desiredStartLocationX, desiredStartLocationY);
        }
    }
}
