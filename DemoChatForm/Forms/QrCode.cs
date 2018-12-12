using System;
using System.Windows.Forms;

namespace DemoChatForm.Forms
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
